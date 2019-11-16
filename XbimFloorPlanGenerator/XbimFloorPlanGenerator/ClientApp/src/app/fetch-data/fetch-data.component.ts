import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-fetch-data',
    templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
    public ifcfiles: IfcFiles[];
    public response: { dbPath: '' };
    private baseUrl: string;
    public uploadFinished = (event) => {
        this.response = event;
    }
    constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
        this.getFiles();
    }

    public getFiles = () => {
        this.http.get<IfcFiles[]>(this.baseUrl + 'ifc').subscribe(result => {
            this.ifcfiles = result;
        }, error => console.error(error));
    }

    public processFile = (fileId) => {
        this.http.post(this.baseUrl + 'ifc', { IfcFileId: fileId })
            .subscribe(event => {
                this.getFiles();
            });
    };
}

interface IfcFiles {
    id: number;
    fileName: string;
    fileSize: number;
    wasProcessed: boolean;
}
