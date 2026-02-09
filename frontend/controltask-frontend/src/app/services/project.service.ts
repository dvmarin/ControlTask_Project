import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ProjectDto } from '../models/project.model';
import { TaskDto, PagedResultDto } from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  constructor(private http: HttpClient) { }

  getProjects(): Observable<ProjectDto[]> {
    return this.http.get<ProjectDto[]>('projects');
  }

  getProject(id: number): Observable<ProjectDto> {
    return this.http.get<ProjectDto>(`projects/${id}`);
  }

getProjectTasks(
  projectId: number,
  pageNumber: number = 1,
  pageSize: number = 10,
  status?: string,
  assignee?: string,
  search?: string,
  sortBy?: string,
  sortDirection?: string
): Observable<any> {
  let params = new HttpParams()
    .set('pageNumber', pageNumber.toString())
    .set('pageSize', pageSize.toString());

  if (status) {
    params = params.set('status', status);
  }
  
  if (assignee) {
    params = params.set('assignee', assignee);
  }
  
  if (search) {
    params = params.set('search', search);
  }
  
  if (sortBy) {
    params = params.set('sortBy', sortBy);
  }
  
  if (sortDirection) {
    params = params.set('sortDirection', sortDirection);
  }

  return this.http.get<any>(`Projects/${projectId}/tasks`, { params });
}

  getAllProjectTasks(
    projectId: number,
    status?: string,
    assigneeId?: number
  ): Observable<TaskDto[]> {
    let url = `projects/${projectId}/tasks/all`;
    
    const params: any = {};
    if (status) params.status = status;
    if (assigneeId) params.assigneeId = assigneeId;
    
    return this.http.get<TaskDto[]>(url, { params });
  }
}