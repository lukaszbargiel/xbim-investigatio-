import { OnInit, Component, ElementRef, NgZone, Input } from '@angular/core';
import { Application, Sprite, Text, Polygon, Point, Graphics } from 'pixi.js';
import { Viewport } from 'pixi-viewport'
import { IfcFloor } from '../floor/floor.component';
@Component({
    selector: 'app-view-port',
    template: ''
})
export class ViewPortComponent implements OnInit {
    
    public app: Application;
    public viewport: Viewport;
    private minX = 99999999;
    private minY = 99999999;
    private maxY = 0;
    private scaleFactor = 1;
    private revertFactor = 1;
    
    constructor(private elementRef: ElementRef, private ngZone: NgZone) { }

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
        let revertFactor = ((this.maxY - this.minY) / this.scaleFactor);
    }
    redraw(floorDefinition: IfcFloor): void {
        
        if (!floorDefinition) {
            return;
        }

        this.setTransformFactors(floorDefinition);
        floorDefinition.walls.forEach((wall) => {

            const productGeometryData = JSON.parse(wall.serializedShapeGeometry);
            this.drawFloorShape(productGeometryData, 0x666666);

            wall.openings.forEach((opening) => {                
                const data = JSON.parse(opening.serializedShapeGeometry);
                this.drawFloorShape(data, 0xe6f2ff);
            });
        });
    }

    drawFloorShape(productGeometryData: any, color?: number) {
        let polygonPoints: Point[] = [];
        
        productGeometryData.forEach((polygonSet) => {
            polygonSet.Polygons.forEach((polygon) => {
                polygonPoints = [];
                polygon.PolygonVertices.forEach((vertice) => {
                    polygonPoints.push(new Point((vertice.X - this.minX) / this.scaleFactor, this.revertFactor - (vertice.Y - this.minY) / this.scaleFactor));
                });

                const ifcPolygon = new Polygon(polygonPoints);                
                const graphics = new Graphics();
                graphics.beginFill(color);
                graphics.drawPolygon(ifcPolygon);

                const text = new Text("WORD");
                
                graphics.addChild(text);
                graphics.interactive = true;                
                graphics.buttonMode = true;
                graphics.on('pointerdown', function (e) {
                    // select
                });
                graphics.on('pointerover', function (e) {
                    // redraw
                });
                
                graphics.endFill();

                this.viewport.addChild(graphics);

            });
        });
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
