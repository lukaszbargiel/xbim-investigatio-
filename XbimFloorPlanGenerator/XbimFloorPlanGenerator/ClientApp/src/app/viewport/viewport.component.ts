import { OnInit, Component, ElementRef, NgZone, Input, OnChanges, SimpleChanges, SimpleChange, Output, EventEmitter, ViewChild, asNativeElements } from '@angular/core';
import { Application, Sprite, Text, Polygon, Point, Graphics, filters, TextStyle } from 'pixi.js';
import { Viewport } from 'pixi-viewport'
import { IfcFloor, IfcSpace } from '../floor/floor.component';
import * as PolyBool from 'polybooljs'
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
        // this can be easily refactored to avoid 3 neste loops, by combining map/reduce methods
        floorDefinition.walls.forEach((wall) => {
            const productGeometryData = JSON.parse(wall.serializedShapeGeometry);
            productGeometryData.forEach((productShape) => {
                productShape.f.forEach((face) => {
                    face.p.forEach((polygon) => {

                        let tmpMinX, tmpMinY = 99999999;
                        let tmpMaxX, tmpMaxY = 0;

                        const minWallX = polygon.pv.reduce(function (prev, curr) {
                            return prev.X < curr.X ? prev : curr;
                        });
                        const minWallY = polygon.pv.reduce(function (prev, curr) {
                            return prev.Y < curr.Y ? prev : curr;
                        });
                        const maxWallX = polygon.pv.reduce(function (prev, curr) {
                            return prev.X > curr.X ? prev : curr;
                        });
                        const maxWallY = polygon.pv.reduce(function (prev, curr) {
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
        });
        debugger;
        let scaleFactorX = (maxX - this.minX) / 1000;
        let scaleFactorY = (this.maxY - this.minY) / 1000;
        this.scaleFactor = Math.max(scaleFactorX, scaleFactorY);
        this.revertFactor = ((this.maxY - this.minY) / this.scaleFactor);
    }

    redraw(floorDefinition: IfcFloor): void {

        if (!floorDefinition) {
            return;
        }
        //PolyBool.epsilon(0.00001);

        this.ifcFloor = floorDefinition;

        this.setTransformFactors(floorDefinition);

        floorDefinition.spaces.forEach((space) => {
            if (!this.spaceColors[space.longName]) {
                this.spaceColors[space.longName] = Math.random() * 0xc0c0c0 << 0;
            }

            const spaceGraphic = this.drawProductV2(space, this.spaceColors[space.longName], 0.5, true);
            
            //const bounds = spaceGraphic.getBounds();

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
            //console.log(stair.ifcId)
            this.drawProductV2(stair, 0x666666);

        });

        floorDefinition.walls.forEach((wall) => {
            this.drawProductV2(wall, 0x666666);
        });

    }

    drawProductV2(ifcProduct: any, color?: number, alpha: number = 1, fill: boolean = false) {
        const productGeometryData = JSON.parse(ifcProduct.serializedShapeGeometry);
        let polygonPoints: Point[] = [];
        // this is important we probably need to calculate it dynamically
        
        const polygonGraphics = new Graphics();
        polygonGraphics.lineStyle(1, 0xcccccc, 1, 1, true);
        if (fill) {
            polygonGraphics.beginFill(color, 0.5);
        }

        var segments = this.generatePolygonSegments(productGeometryData);

        if (ifcProduct.openings) {
            ifcProduct.openings.forEach((opening) => {
                const data = JSON.parse(opening.serializedShapeGeometry);
                const openingGraphicSegments = this.generatePolygonSegments(data);
                var combination = PolyBool.combine(segments, openingGraphicSegments);
                // cut off the opening from the wall polygon + to draw the opening conside using PolyBool.selectIntersect
                segments = PolyBool.selectDifference(combination);
                // don't want to draw it - just want to cut it off the wall
                //this.viewport.addChild(openingGraphic);
            });
        }
        var resultPolygon = PolyBool.polygon(segments);

        if (resultPolygon.regions.length === 0) {
            return null;
        }
        resultPolygon.regions.forEach((region) => {
            polygonPoints = [];
            region.forEach((point) => {                
                polygonPoints.push(new Point((point[0] - this.minX) / this.scaleFactor, this.revertFactor - (point[1] - this.minY) / this.scaleFactor));
            });
            const ifcPolygon = new Polygon(polygonPoints);
            polygonGraphics.drawPolygon(ifcPolygon);
        });
        
        polygonGraphics.interactive = true;
        polygonGraphics.buttonMode = true;
        this.viewport.addChild(polygonGraphics);
        polygonGraphics.endFill();
        return polygonGraphics;
    }

    generatePolygonSegments(productGeometryData: any): any {
        
        let firstPolygon = {
            regions: [],
            inverted: false
        }
        var segments = PolyBool.segments(firstPolygon);

        productGeometryData.forEach((productShape) => {
                       
            productShape.f.forEach((face) => {
                face.p.forEach((polygon) => {

                    var nextPolygon = {
                        regions: [],
                        inverted: false
                    }

                    nextPolygon.regions.push([]);

                    polygon.pv.forEach((vertice) => {
                        nextPolygon.regions[0].push([vertice.X, vertice.Y]);
                    });
                    var nextSegment = PolyBool.segments(nextPolygon);

                    var comb = PolyBool.combine(segments, nextSegment);
                    segments = PolyBool.selectUnion(comb);
                });
            });
            
        });
        return segments;
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
