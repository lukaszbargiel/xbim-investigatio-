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
                this.drawFloorPlan()
            }, error => console.error(error));
        });
        this.ctx.canvas.addEventListener('mousemove', this.selectRoomOnMouseOver);
    }

    selectRoomOnMouseOver = (e) => {
        const mousePos = {
            x: e.clientX - this.ctx.canvas.offsetLeft,
            y: e.clientY - this.ctx.canvas.offsetTop
        };
        console.log(mousePos.x + ',' + mousePos.y);
    }

    selectSpaceOnCanvas() {
        const data = JSON.parse(this.selectedSpace.spaceCoordinates);

        this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
        this.drawFloorPlan();
        data.forEach((coordinates) => {
            this.ctx.fillStyle = '#f00';
            if (coordinates.SpacePositionType == 0) {
                this.ctx.beginPath();
                this.ctx.moveTo((coordinates.SweptAreaCoordinates[0].X - this.minX) / this.scaleFactor, (coordinates.SweptAreaCoordinates[0].Y - this.minY) / this.scaleFactor);
                coordinates.SweptAreaCoordinates.forEach((sweptCoord) => {
                    this.ctx.lineTo((sweptCoord.X - this.minX) / this.scaleFactor, (sweptCoord.Y - this.minY) / this.scaleFactor);
                });
                this.ctx.closePath();

            }
            else if (coordinates.SpacePositionType == 1) {
                // x is a point in the middle so we need to add half of its size to position
                const startX = coordinates.X - coordinates.XDim / 2;
                const startY = coordinates.Y - coordinates.YDim / 2;
                this.ctx.fillRect((startX - this.minX) / this.scaleFactor, (startY - this.minY) / this.scaleFactor, coordinates.XDim / this.scaleFactor, coordinates.YDim / this.scaleFactor);
            }
            this.ctx.fill();
        });
    }

    drawFloorPlan() {
        this.ctx.fillStyle = 'black';
        let maxX = 0;
        let maxY = 0;
        this.ifcFloor.walls.forEach((wall) => {
            wall.productShapes.forEach((shape) => {
                if (this.minX > shape.boundingBoxX) {
                    this.minX = shape.boundingBoxX;
                }
                if (maxX < (shape.boundingBoxX + shape.boundingBoxSizeX)) {
                    maxX = (shape.boundingBoxX + shape.boundingBoxSizeX);
                }
                if (this.minY > shape.boundingBoxY) {
                    this.minY = shape.boundingBoxY;
                }
                if (maxY < (shape.boundingBoxY + shape.boundingBoxSizeY)) {
                    maxY = (shape.boundingBoxY + shape.boundingBoxSizeY);
                }
            });
        });

        let scaleFactorX = (maxX - this.minX)/1000;
        let scaleFactorY = (maxY - this.minY) / 1000;
        this.scaleFactor = Math.max(scaleFactorX, scaleFactorY);
        this.ifcFloor.walls.forEach((wall) => {
            wall.productShapes.forEach((shape) => {
                let x = (shape.boundingBoxX - this.minX) / this.scaleFactor;
                let y = (shape.boundingBoxY - this.minY) / this.scaleFactor;
                // move all points to positive axis
                if (this.minX < 0) {
                    x = x - this.minX;
                }
                if (this.minY < 0) {
                    y = y - this.minY;
                }
                const sizex = shape.boundingBoxSizeX / this.scaleFactor;
                const sizey = shape.boundingBoxSizeY / this.scaleFactor;

                this.ctx.strokeRect(x, y, sizex, sizey);
            });
        });

        const sampleSpace = this.ifcFloor.spaces.find(x => x.spaceCoordinates !== null);
        //this.selectSpaceOnCanvas(sampleSpace);
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
}

interface IfcSpace {
    id: number;
    entityLabel: string;
    description: string;
    productShapes: IfcProductShape[];
    grossFloorArea: number;
    ifcName: string;
    netFloorArea: number;
    spaceCoordinates: string;
}

interface IfcProductShape {
    id: number;
    shapeType: number;
    boundingBoxX: number;    
    boundingBoxY: number;
    boundingBoxSizeX: number;
    boundingBoxSizeY: number;
}
