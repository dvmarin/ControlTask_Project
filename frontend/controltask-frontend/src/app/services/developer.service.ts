import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { DeveloperDto } from '../models/developer.model';

@Injectable({
  providedIn: 'root'
})
export class DeveloperService {
  constructor(private http: HttpClient) { }

  getActiveDevelopers(): Observable<DeveloperDto[]> {
    return this.http.get<DeveloperDto[]>('developers');
  }

  getDeveloper(id: number): Observable<DeveloperDto> {
    return this.http.get<DeveloperDto>(`developers/${id}`);
  }
}