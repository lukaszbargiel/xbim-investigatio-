import { OnInit, Component, ElementRef, NgZone, Input } from '@angular/core';
import { Application, Sprite, Texture, Polygon, Point, Graphics } from 'pixi.js';
import { Viewport } from 'pixi-viewport'
import { IfcFloor } from '../floor/floor.component';
@Component({
    selector: 'app-view-port',
    template: ''
})
export class ViewPortComponent implements OnInit {
    
    public app: Application;
    public viewport: Viewport;
    public polygons = [{ "Polygons": [{ "PolygonVertices": [{ "X": 2.5999999046325684, "Y": -0.4169999957084656 }, { "X": 8.383000373840332, "Y": -0.4169999957084656 }, { "X": 8.383000373840332, "Y": -5.199999809265137 }, { "X": 2.5999999046325684, "Y": -5.199999809265137 }] }, { "PolygonVertices": [{ "X": 2.5999999046325684, "Y": -0.4169999957084656 }, { "X": 8.383000373840332, "Y": -0.4169999957084656 }, { "X": 8.383000373840332, "Y": -5.199999809265137 }, { "X": 2.5999999046325684, "Y": -5.199999809265137 }] }] }]
    constructor(private elementRef: ElementRef, private ngZone: NgZone) { }

    redraw(floorDefinition: IfcFloor): void {
        let polygonPoints: Point[] = [];
        if (!floorDefinition) {
            return;
        }
        
        floorDefinition.walls.forEach((wall) => {

            const productGeometryData = JSON.parse(wall.serializedShapeGeometry);
            productGeometryData.forEach((polygonSet) => {
                polygonSet.Polygons.forEach((polygon) => {
                    polygonPoints = [];
                    polygon.PolygonVertices.forEach((vertice) => {
                        polygonPoints.push(new Point(vertice.X, vertice.Y));
                    });
                    const graphics = new Graphics();
                    graphics.beginFill(0xcccccc);
                    graphics.drawPolygon(polygonPoints);
                    graphics.endFill();

                    this.viewport.addChild(graphics);

                });
            });
        });
    }
    ngOnInit(): void {
        this.ngZone.runOutsideAngular(() => {
            this.app = new Application({ backgroundColor: 0xffffff });
        });
        this.elementRef.nativeElement.appendChild(this.app.view);

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

            // add a red box
            const sprite = this.viewport.addChild(new Sprite(Texture.WHITE))
            sprite.tint = 0xff0000
            sprite.width = sprite.height = 100
            sprite.position.set(10, 20)
        }
}
