import { Component, Inject, ViewChild, ElementRef, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { ViewPortComponent } from '../viewport/viewport.component';
@Component({
    selector: 'app-floor-details',
    templateUrl: './floor.component.html'
})
export class FloorComponent implements OnInit  {
    @ViewChild('viewport', { static: true }) private viewPort: ViewPortComponent;
    
    public ifcFloor: IfcFloor;

    private baseUrl: string;
    private spaceCategoriesColors: { [spaceName: string]: number };

    selectedSpace: IfcSpace;
    selectedBoundries: IfcBoundry[];

    constructor(
        private route: ActivatedRoute,
        private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    ngOnInit() {
        this.route.paramMap.subscribe(params => {
            this.http.get<IfcFloor>(this.baseUrl + 'floor-plan/' + params.get('id')).subscribe(result => {
                this.ifcFloor = result;
                this.viewPort.redraw(this.ifcFloor);
                
            }, error => console.error(error));
        });
    }

    selectSpace(spaceId: number) {
        const selectingSpace = this.ifcFloor.spaces.find(space => space.id === spaceId);
        this.selectedSpace = selectingSpace;
        this.selectedBoundries = <IfcBoundry[]>JSON.parse(selectingSpace.serializedBoundryData);
    }

    colorCategories(spaceColors: { [spaceName: string]: number }) {
        this.spaceCategoriesColors = spaceColors;
    }

}

export interface IfcFloor {
    id: number;
    entityLabel: string;
    description: string;
    walls: IfcWall[];
    stairs: IfcStair[];
    spaces: IfcSpace[];
}
interface IfcStair {
    id: number;
    entityLabel: string;
    description: string;
    productShapes: IfcProductShape[]
    serializedShapeGeometry: string;
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

interface IfcBoundry {
    IsPhysical: boolean;
    IsExternal: boolean;
    BoundryType: string;
    BounderyArea: number;
    BoundryName: string;
}
export interface IfcSpace {
    id: number;
    entityLabel: string;
    description: string;
    productShapes: IfcProductShape[];
    grossFloorArea: number;
    ifcName: string;
    longName: string;
    netFloorArea: number;
    serializedShapeGeometry: string;
    serializedBoundryData: string;
}

interface IfcProductShape {
    id: number;
    shapeType: number;
    boundingBoxX: number;
    boundingBoxY: number;
    boundingBoxSizeX: number;
    boundingBoxSizeY: number;
}
