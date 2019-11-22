import { Component, Inject, ViewChild, ElementRef, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { ViewPortComponent } from '../viewport/viewport.component';
@Component({
    selector: 'app-floor-details',
    templateUrl: './floor.component.html'
})
export class FloorComponent implements OnInit  {
    @ViewChild('canvas', { static: true }) canvas: ElementRef<HTMLCanvasElement>;
    @ViewChild('viewport', { static: true }) private viewPort: ViewPortComponent;
    
    public ifcFloor: IfcFloor;

    private baseUrl: string;
    private minX = 99999999;
    private minY = 99999999;
    private maxY = 0;
    private scaleFactor = 1;

    selectedSpace: any;

    constructor(
        private route: ActivatedRoute,
        private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    private ctx: CanvasRenderingContext2D;

    ngOnInit() {
        this.ctx = this.canvas.nativeElement.getContext('2d');
        this.route.paramMap.subscribe(params => {
            this.http.get<IfcFloor>(this.baseUrl + 'floor-plan/' + params.get('id')).subscribe(result => {
                this.ifcFloor = result;
                this.setTransformFactors();
                this.drawFloorPlan();
                this.viewPort.redraw(this.ifcFloor);
                
            }, error => console.error(error));
        });
        //this.ctx.canvas.addEventListener('mousemove', this.selectRoomOnMouseOver);
    }

    selectRoomOnMouseOver = (e) => {
        const mousePos = {
            x: e.clientX - this.ctx.canvas.offsetLeft,
            y: e.clientY - this.ctx.canvas.offsetTop
        };
        //console.log(mousePos.x + ',' + mousePos.y);
    }

    selectSpaceOnCanvas() {
        const data = JSON.parse(this.selectedSpace.serializedShapeGeometry);

        this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
        this.drawFloorPlan();
        this.ctx.beginPath();
        this.ctx.fillStyle = '#9cf';
        this.drawProduct(data);
        this.ctx.closePath();
        this.ctx.fill();

    }

    setTransformFactors() {
        let maxX = 0;
        this.maxY = 0;

        this.ifcFloor.walls.forEach((wall) => {
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
    }
    drawFloorPlan() {
        //this.ctx.fillStyle = 'black';

        this.ifcFloor.walls.forEach((wall) => {
            //if (wall.id != 2662) {
            //    return;
            //}
            //debugger;
            const data = JSON.parse(wall.serializedShapeGeometry);

            //this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
            //this.drawFloorPlan();
            this.ctx.fillStyle = '#666666';
            this.drawProduct(data);
            wall.openings.forEach((opening) => {
                this.ctx.fillStyle = '#e6f2ff';
                const data = JSON.parse(opening.serializedShapeGeometry);
                this.drawProduct(data);
            });
        });

    }

    drawProduct(productGeometryData: any) {
        var revertFactor = ((this.maxY - this.minY) / this.scaleFactor);
        productGeometryData.forEach((polygonSet) => {
            polygonSet.Polygons.forEach((polygon) => {
                this.ctx.strokeStyle = '#' + (Math.random() * 0xFFFFFF << 0).toString(16);
                this.ctx.beginPath();
                // since Y axis starts at the bottom of the canvas which is in opposite to IFC we need to cast Y coordinates based on formulat Ynew = YMax - Y
                this.ctx.moveTo((polygon.PolygonVertices[0].X - this.minX) / this.scaleFactor, revertFactor - (polygon.PolygonVertices[0].Y - this.minY) / this.scaleFactor);
                polygon.PolygonVertices.forEach((vertice) => {
                    this.ctx.lineTo((vertice.X - this.minX) / this.scaleFactor, revertFactor - (vertice.Y - this.minY) / this.scaleFactor);
                });
                
                this.ctx.closePath();
                this.ctx.fill();

                //this.ctx.fill();
            });
        });
    }
}

export interface IfcFloor {
    id: number;
    entityLabel: string;
    description: string;
    walls: IfcWall[];
    spaces: IfcSpace[];
}
interface IfcWall {
    id: number;
    entityLabel: string;
    description: string;
    openings: IfcOpening[]
    productShapes: IfcProductShape[]
    serializedShapeGeometry: string;
}

interface IfcOpening {
    id: number;
    entityLabel: string;
    description: string;
    ifcName: string;        
    serializedShapeGeometry: string;
}

interface IfcSpace {
    id: number;
    entityLabel: string;
    description: string;
    productShapes: IfcProductShape[];
    grossFloorArea: number;
    ifcName: string;
    longName: string;
    netFloorArea: number;
    serializedShapeGeometry: string;
}

interface IfcProductShape {
    id: number;
    shapeType: number;
    boundingBoxX: number;
    boundingBoxY: number;
    boundingBoxSizeX: number;
    boundingBoxSizeY: number;
}
