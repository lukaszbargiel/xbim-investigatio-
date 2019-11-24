import { OnInit, Component, ElementRef, NgZone, Input, OnChanges, SimpleChanges, SimpleChange, Output, EventEmitter } from '@angular/core';
import { Application, Sprite, Text, Polygon, Point, Graphics, filters, TextStyle } from 'pixi.js';
import { Viewport } from 'pixi-viewport'
import { IfcFloor, IfcSpace } from '../floor/floor.component';
@Component({
    selector: 'app-view-port',
    template: ''
})
export class ViewPortComponent implements OnInit, OnChanges  {
    @Input('selected-space') selectedSpace: IfcSpace;
    public app: Application;
    public viewport: Viewport;
    private minX = 99999999;
    private minY = 99999999;
    private maxY = 0;
    private scaleFactor = 1;
    private revertFactor = 1;
    public spaceGraphics: { [id: number]: Graphics; } = {};
    public spaceLabelStyle = new TextStyle({
        fontSize: 36
    });

    constructor(private elementRef: ElementRef, private ngZone: NgZone) { }
    @Output() selectSpaceEvent = new EventEmitter<number>();

    ngOnChanges(changes: SimpleChanges) {
        const currentItem: SimpleChange = changes.selectedSpace;
        if (currentItem.previousValue) {
            const unselectedSpaceGraphic = this.spaceGraphics[currentItem.previousValue.id];
            const colorMatrix = this.getColorMatrix(0xffffff);
            unselectedSpaceGraphic.filters = [colorMatrix];        
        }
        if (currentItem.currentValue) {
            const selectedSpaceGraphic = this.spaceGraphics[currentItem.currentValue.id];                    
            const colorMatrix = this.getColorMatrix(0x94ffa8);
            selectedSpaceGraphic.filters = [colorMatrix];        
        }

    }
    
    // This method let us to calculate factors that are later use to scale the given points to match canvas    
    setTransformFactors(floorDefinition: IfcFloor) {
        let maxX = 0;
        this.maxY = 0;

        floorDefinition.walls.forEach((wall) => {
            const productGeometryData = JSON.parse(wall.serializedShapeGeometry);
            productGeometryData.forEach((polygonSet) => {
                polygonSet.Polygons.forEach((polygon) => {

                    let tmpMinX, tmpMinY = 99999999;
                    let tmpMaxX, tmpMaxY = 0;

                    const minWallX = polygon.PolygonVertices.reduce(function (prev, curr) {
                        return prev.X < curr.X ? prev : curr;
                    });
                    const minWallY = polygon.PolygonVertices.reduce(function (prev, curr) {
                        return prev.Y < curr.Y ? prev : curr;
                    });
                    const maxWallX = polygon.PolygonVertices.reduce(function (prev, curr) {
                        return prev.X > curr.X ? prev : curr;
                    });
                    const maxWallY = polygon.PolygonVertices.reduce(function (prev, curr) {
                        return prev.Y > curr.Y ? prev : curr;
                    });
                    tmpMinX = minWallX.X;
                    tmpMinY = minWallY.Y
                    tmpMaxX = maxWallX.X;
                    tmpMaxY = maxWallY.Y;



                    if (this.minX > tmpMinX) {
                        this.minX = tmpMinX;
                    }
                    if (maxX < tmpMaxX) {
                        maxX = tmpMaxX;
                    }
                    if (this.minY > tmpMinY) {
                        this.minY = tmpMinY;
                    }
                    if (this.maxY < tmpMaxY) {
                        this.maxY = tmpMaxY;
                    }
                });

            });
        });
        let scaleFactorX = (maxX - this.minX) / 1000;
        let scaleFactorY = (this.maxY - this.minY) / 1000;
        this.scaleFactor = Math.max(scaleFactorX, scaleFactorY);
        this.revertFactor = ((this.maxY - this.minY) / this.scaleFactor);
    }

    redraw(floorDefinition: IfcFloor): void {

        if (!floorDefinition) {
            return;
        }

        this.setTransformFactors(floorDefinition);

        floorDefinition.walls.forEach((wall) => {
            const productGeometryData = JSON.parse(wall.serializedShapeGeometry);
            const wallGraphic = this.generateGraphics(productGeometryData, 0x666666, wall.id);
            this.viewport.addChild(wallGraphic);

            wall.openings.forEach((opening) => {
                const data = JSON.parse(opening.serializedShapeGeometry);
                const openingGraphic = this.generateGraphics(data, 0xe6f2ff);
                this.viewport.addChild(openingGraphic);
            });
        });

        floorDefinition.spaces.forEach((space) => {
            const productGeometryData = JSON.parse(space.serializedShapeGeometry);
            const spaceGraphic = this.generateGraphics(productGeometryData, 0xffffff, space.id);
            const bounds = spaceGraphic.getBounds();

            // we cannot simply add text - it needs to be adjust to container size
            const spaceName = new Text(space.longName, this.spaceLabelStyle);
            while (spaceName.width > bounds.width) spaceName.style.fontSize--;
            spaceName.x = bounds.x + (bounds.width - spaceName.width) /2;
            spaceName.y = bounds.y + bounds.height / 2;
            spaceGraphic.addChild(spaceName);

            spaceGraphic.on('pointerover', (e) => {
                const colorMatrix = this.getColorMatrix(0x94ffa8);
                e.currentTarget.filters = [colorMatrix];
            });
            spaceGraphic.on('pointerout', (e) => {
                const assignedSpaceId = Object.keys(this.spaceGraphics).find(key => this.spaceGraphics[key] === e.currentTarget);
                if (this.selectedSpace && this.selectedSpace.id === Number(assignedSpaceId)) {
                    return;
                }
                e.currentTarget.filters = null;
            });

            spaceGraphic.on('pointerdown', (e) => {
                const selectedSpaceId : number = Number(Object.keys(this.spaceGraphics).find(key => this.spaceGraphics[key] === e.currentTarget));
                this.selectSpaceEvent.next(selectedSpaceId);
                
            });
            this.viewport.addChild(spaceGraphic);

            this.spaceGraphics[space.id] = spaceGraphic;
        });

    }

    generateGraphics(productGeometryData: any, color?: number, shapeId?: number): Graphics {
        let polygonPoints: Point[] = [];

        const polygonGraphics = new Graphics();
        polygonGraphics.beginFill(color);

        productGeometryData.forEach((polygonSet) => {

            polygonSet.Polygons.forEach((polygon) => {
                polygonPoints = [];
                polygon.PolygonVertices.forEach((vertice) => {
                    polygonPoints.push(new Point((vertice.X - this.minX) / this.scaleFactor, this.revertFactor - (vertice.Y - this.minY) / this.scaleFactor));
                });

                const ifcPolygon = new Polygon(polygonPoints);
                polygonGraphics.drawPolygon(ifcPolygon);
                polygonGraphics.interactive = true;
                polygonGraphics.buttonMode = true;

            });

        });

        polygonGraphics.endFill();

        return polygonGraphics;

    }

    getColorMatrix(tintColor: any): filters.ColorMatrixFilter {
        const color = new filters.ColorMatrixFilter();
        const tint = tintColor;
        const r = tint >> 16 & 0xFF;
        const g = tint >> 8 & 0xFF;
        const b = tint & 0xFF;
        color.matrix[0] = r / 255;
        color.matrix[6] = g / 255;
        color.matrix[12] = b / 255;
        return color;
    }
    ngOnInit(): void {
        this.ngZone.runOutsideAngular(() => {
            this.app = new Application({ backgroundColor: 0xffffff });
        });

        this.elementRef.nativeElement.appendChild(this.app.view);

        this.elementRef.nativeElement.addEventListener("wheel", function (event) {
            event.preventDefault()
        });


        this.viewport = new Viewport({
            screenWidth: parent.innerWidth,
            screenHeight: window.innerHeight,
            worldWidth: 1000,
            worldHeight: 1000,
            interaction: this.app.renderer.plugins.interaction // the interaction module is important for wheel to work properly when renderer.view is placed or scaled
        })

        this.app.stage.addChild(this.viewport);

        // activate plugins
        this.viewport
            .drag()
            .pinch()
            .wheel()
            .decelerate()


    }


}
