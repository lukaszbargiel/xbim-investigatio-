import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IfcFloor } from '../floor/floor.component';

@Component({
    selector: 'app-projects',
    templateUrl: './projects.component.html'
})
export class ProjectsComponent {
    public ifcProjects: IfcProject[];
    public selectedProject: IfcProject;

    private baseUrl: string;
    constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
        this.getProjects();
    }

    public getProjects = () => {
        this.http.get<IfcProject[]>(this.baseUrl + 'project').subscribe(result => {
            this.ifcProjects = result;
        }, error => console.error(error));
    }

    public selectProject = (project) => {
        this.selectedProject = project;
    };
}

interface IfcProject {
    id: number;
    entityLabel: string;
    sites: IfcSite[]
}
interface IfcSite {
    id: number;
    entityLabel: string;
    description: string;
    buildings: IfcBuilding[]
}
interface IfcBuilding {
    id: number;
    entityLabel: string;
    description: string;
    floors: IfcFloor[]
}
