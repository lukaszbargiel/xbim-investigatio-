import { Component, Inject, ViewChild, ElementRef, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
@Component({
    selector: 'app-floor-details',
    templateUrl: './floor.component.html'
})
export class FloorComponent implements OnInit {
    @ViewChild('canvas', { static: true })
    canvas: ElementRef<HTMLCanvasElement>;  
    public ifcFloor: IfcFloor;

    private baseUrl: string;
    private minX = 99999999;
    private minY = 99999999;
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
                this.drawFloorPlan()
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
        
        this.drawProduct(data);
        this.ctx.closePath();
        this.ctx.fill();

    }

    setTransformFactors() {
        let maxX = 0;
        let maxY = 0;
        this.ifcFloor.walls.forEach((wall) => {
            const productGeometryData = JSON.parse(wall.serializedShapeGeometry);
            productGeometryData.forEach((geometryData) => {
                let tmpMinX, tmpMinY = 99999999;
                let tmpMaxX, tmpMaxY = 0;
                if (geometryData.ShapeGeometryType == 0) {
                    const minWallX = geometryData.ShapeVertices.reduce(function (prev, curr) {
                        return prev.X < curr.X ? prev : curr;
                    });
                    const minWallY = geometryData.ShapeVertices.reduce(function (prev, curr) {
                        return prev.Y < curr.Y ? prev : curr;
                    });
                    const maxWallX = geometryData.ShapeVertices.reduce(function (prev, curr) {
                        return prev.X > curr.X ? prev : curr;
                    });
                    const maxWallY = geometryData.ShapeVertices.reduce(function (prev, curr) {
                        return prev.Y > curr.Y ? prev : curr;
                    });
                    tmpMinX = minWallX.X;
                    tmpMinY = minWallY.Y
                    tmpMaxX = maxWallX.X;
                    tmpMaxY = maxWallY.Y;

                }
                else if (geometryData.ShapeGeometryType == 1) {
                    // x is a point in the middle so we need to add half of its size to position
                    tmpMinX = geometryData.X - geometryData.XDim / 2;
                    tmpMinY = geometryData.Y - geometryData.YDim / 2;
                    tmpMaxX = geometryData.X + geometryData.XDim / 2;
                    tmpMaxY = geometryData.Y + geometryData.YDim / 2; 
                }

                if (this.minX > tmpMinX) {
                    this.minX = tmpMinX;
                }
                if (maxX < tmpMaxX) {
                    maxX = tmpMaxX;
                }
                if (this.minY > tmpMinY) {
                    this.minY = tmpMinY;
                }
                if (maxY < tmpMaxY) {
                    maxY = tmpMaxY;
                }
            });

        });

        let scaleFactorX = (maxX - this.minX) / 1000;
        let scaleFactorY = (maxY - this.minY) / 1000;
        this.scaleFactor = Math.max(scaleFactorX, scaleFactorY);
    }
    drawFloorPlan() {
        //this.ctx.fillStyle = 'black';
        this.ifcFloor.walls.forEach((wall) => {
            const data = JSON.parse(wall.serializedShapeGeometry);

            //this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
            //this.drawFloorPlan();
            this.drawProduct(data);
        });

        //let scaleFactorX = (maxX - this.minX)/1000;
        //let scaleFactorY = (maxY - this.minY) / 1000;
        //this.scaleFactor = Math.max(scaleFactorX, scaleFactorY);
        //this.ifcFloor.walls.forEach((wall) => {
        //    wall.productShapes.forEach((shape) => {
        //        let x = (shape.boundingBoxX - this.minX) / this.scaleFactor;
        //        let y = (shape.boundingBoxY - this.minY) / this.scaleFactor;
        //        // move all points to positive axis
        //        if (this.minX < 0) {
        //            x = x - this.minX;
        //        }
        //        if (this.minY < 0) {
        //            y = y - this.minY;
        //        }
        //        const sizex = shape.boundingBoxSizeX / this.scaleFactor;
        //        const sizey = shape.boundingBoxSizeY / this.scaleFactor;

        //        this.ctx.strokeRect(x, y, sizex, sizey);
        //    });
        //});

        //const sampleSpace = this.ifcFloor.spaces.find(x => x.spaceCoordinates !== null);
        //this.selectSpaceOnCanvas(sampleSpace);
    }

    drawProduct(productGeometryData: any) {
        productGeometryData.forEach((geometryData) => {
            this.ctx.strokeStyle = '#' + (Math.random() * 0xFFFFFF << 0).toString(16);
            if (geometryData.ShapeGeometryType == 0) {
                this.ctx.beginPath();
                this.ctx.moveTo((geometryData.ShapeVertices[0].X - this.minX) / this.scaleFactor, (geometryData.ShapeVertices[0].Y - this.minY) / this.scaleFactor);
                geometryData.ShapeVertices.forEach((sweptCoord) => {
                    this.ctx.lineTo((sweptCoord.X - this.minX) / this.scaleFactor, (sweptCoord.Y - this.minY) / this.scaleFactor);
                });
                this.ctx.closePath();
                this.ctx.stroke();
            }
            else if (geometryData.ShapeGeometryType == 1) {
                // x is a point in the middle so we need to add half of its size to position
                const startX = geometryData.X - geometryData.XDim / 2;
                const startY = geometryData.Y - geometryData.YDim / 2;                
                const rectangleX = (startX - this.minX) / this.scaleFactor;
                const rectangleY = (startY - this.minY) / this.scaleFactor;
                
                this.ctx.strokeRect(rectangleX, rectangleY, geometryData.XDim / this.scaleFactor, geometryData.YDim / this.scaleFactor);                
            }
            //this.ctx.fill();
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
    productShapes: IfcProductShape[]
    serializedShapeGeometry: string;
}

interface IfcSpace {
    id: number;
    entityLabel: string;
    description: string;
    productShapes: IfcProductShape[];
    grossFloorArea: number;
    ifcName: string;
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
