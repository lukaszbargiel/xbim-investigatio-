import { OnInit, Component, ElementRef, NgZone, Input, OnChanges, SimpleChanges, SimpleChange, Output, EventEmitter, ViewChild, asNativeElements } from '@angular/core';
import { Application, Sprite, Text, Polygon, Point, Graphics, filters, TextStyle } from 'pixi.js';
import { Viewport } from 'pixi-viewport'
import { IfcFloor, IfcSpace } from '../floor/floor.component';
@Component({
    selector: 'app-view-port',
    template: ''
})
export class ViewPortComponent implements OnInit, OnChanges {
    @Input('selected-space') selectedSpace: IfcSpace;

    public app: Application;
    public viewport: Viewport;
    private minX = 99999999;
    private minY = 99999999;
    private maxY = 0;
    private scaleFactor = 1;
    private revertFactor = 1;
    private ifcFloor: IfcFloor;
    private spaceColors: { [spaceName: string]: number; } = {};

    public spaceGraphics: { [id: number]: Graphics; } = {};
    public spaceLabelStyle = new TextStyle({
        fontSize: 12,
        wordWrap: true
    });

    constructor(private elementRef: ElementRef, private ngZone: NgZone) { }
    @Output() selectSpaceEvent = new EventEmitter<number>();
    @Output() colorCategoriesEvent = new EventEmitter<{ [spaceName: string]: number; }>();
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
        this.ifcFloor = floorDefinition;

        this.setTransformFactors(floorDefinition);

        floorDefinition.spaces.forEach((space) => {
            if (!this.spaceColors[space.longName]) {
                this.spaceColors[space.longName] = Math.random() * 0xc0c0c0 << 0;
            }
            const productGeometryData = JSON.parse(space.serializedShapeGeometry);
            const spaceGraphic = this.generateGraphics(productGeometryData, this.spaceColors[space.longName], 0.5);
            const bounds = spaceGraphic.getBounds();

            // we cannot simply add text - it needs to be adjust to container size
            // add space name
            //const spaceName = new Text(space.longName, this.spaceLabelStyle);
            //spaceName.resolution = 4;
            //spaceName.style.wordWrap = true;
            //spaceName.style.wordWrapWidth = bounds.width;

            ////let fontSize = spaceName.style.fontSize;
            ////while (spaceName.width > bounds.width) spaceName.style.fontSize--;

            //spaceName.style.fontSize = 12 / this.viewport.scale.x;
            //if (spaceName.width < bounds.width)  {
            //    //while (spaceName.width > (bounds.width * this.viewport.scale.x)) spaceName.style.fontSize--;
            //    //
            //    spaceName.x = bounds.x + (bounds.width - spaceName.width) / 2;
            //    spaceName.y = bounds.y + bounds.height / 2;
            //    spaceGraphic.addChild(spaceName);
            //}
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
                const selectedSpaceId: number = Number(Object.keys(this.spaceGraphics).find(key => this.spaceGraphics[key] === e.currentTarget));
                this.selectSpaceEvent.next(selectedSpaceId);

            });
            this.viewport.addChild(spaceGraphic);

            this.spaceGraphics[space.id] = spaceGraphic;
        });
        this.colorCategoriesEvent.next(this.spaceColors);

        floorDefinition.stairs.forEach((stair) => {
            const productGeometryData = JSON.parse(stair.serializedShapeGeometry);
            this.drawStairs(productGeometryData, 0x666666);

        });

        floorDefinition.walls.forEach((wall) => {
            const productGeometryData = JSON.parse(wall.serializedShapeGeometry);
            const wallGraphic = this.generateGraphics(productGeometryData, 0x666666);
            this.viewport.addChild(wallGraphic);

            wall.openings.forEach((opening) => {
                const data = JSON.parse(opening.serializedShapeGeometry);
                const openingGraphic = this.generateGraphics(data, 0xe6f2ff);
                this.viewport.addChild(openingGraphic);
            });
        });

    }

    drawStairs(productGeometryData: any, color?: number, alpha: number = 1) {
        let polygonPoints: Point[] = [];

        productGeometryData.forEach((polygonSet) => {
            var sortedVertices = this.sortVertices(polygonSet.Polygons);

            const polygonGraphics = new Graphics();
            polygonGraphics.lineStyle(1, 0xD5402B, 1, 1, true);
            //polygonGraphics.beginFill(stairColor);
            polygonPoints = [];
            sortedVertices.forEach((vertice) => {
                polygonPoints.push(new Point((vertice.X - this.minX) / this.scaleFactor, this.revertFactor - (vertice.Y - this.minY) / this.scaleFactor));
            });

            const ifcPolygon = new Polygon(polygonPoints);
            polygonGraphics.drawPolygon(ifcPolygon);
            polygonGraphics.interactive = true;
            polygonGraphics.buttonMode = true;
            //polygonGraphics.endFill();
            this.viewport.addChild(polygonGraphics);



        });

    }

    sortVertices(polygons: any[]): any[] {
        var x = 0,
            y = 0,
            i,
            j,
            f,
            point1,
            point2;
        var vertices = polygons.map(p => p.PolygonVertices).reduce(function (a, b) { return a.concat(b); });
        var distinctVertices = [...new Set(vertices.map(o => JSON.stringify(o)))].map(o => JSON.parse(String(o)));

        for (i = 0, j = distinctVertices.length - 1; i < distinctVertices.length; j = i, i++) {
            point1 = distinctVertices[i];
            point2 = distinctVertices[j];
            f = point1.X * point2.Y - point2.X * point1.Y;
            x += (point1.X + point2.X) * f;
            y += (point1.Y + point2.Y) * f;
        }

        f = this.calculateArea(distinctVertices) * 6;
        //debugger;
        var center = f == 0 ? new Point(x / f, y / f) : new Point(0,0);
        var startAng;
        distinctVertices.forEach(point => {
            var ang = Math.atan2(point.Y - center.y, point.X - center.x);
            if (!startAng) { startAng = ang }
            else {
                if (ang < startAng) {  // ensure that all points are clockwise of the start point
                    ang += Math.PI * 2;
                }
            }
            point.angle = ang; // add the angle to the point
        });


        // Sort clockwise;
        return distinctVertices.sort((a, b) => a.angle - b.angle);
    };

    calculateArea(vertices: any[]) {
        var area = 0,
            i,
            j,
            point1,
            point2;

        for (i = 0, j = vertices.length - 1; i < vertices.length; j = i, i++) {
            point1 = vertices[i];
            point2 = vertices[j];
            area += point1.X * point2.Y;
            area -= point1.Y * point2.X;
        }
        area /= 2;

        return area;
    };


    generateGraphics(productGeometryData: any, color?: number, alpha: number = 1): Graphics {
        let polygonPoints: Point[] = [];

        const polygonGraphics = new Graphics();
        //polygonGraphics.lineStyle(1, 0xD5402B, 1);
        polygonGraphics.beginFill(color);

        productGeometryData.forEach((polygonSet) => {

            polygonSet.Polygons.forEach((polygon) => {
                polygonPoints = [];
                var i = 0;
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
            this.app = new Application({
                backgroundColor: 0xffffff,
                resizeTo: this.elementRef.nativeElement.parentElement,
                resolution: devicePixelRatio
            });
        });

        this.elementRef.nativeElement.appendChild(this.app.view);

        this.elementRef.nativeElement.addEventListener("wheel", e => {
            //if (this.ifcFloor) {                
            //    this.redraw(this.ifcFloor);
            //}
            e.preventDefault()
        });

        this.viewport = new Viewport({
            screenWidth: window.innerWidth,
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
