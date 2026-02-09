import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { DeveloperWorkloadDto, DeveloperDelayRiskDto } from '../models/developer.model';
import { ProjectHealthDto } from '../models/project.model';
import { UpcomingTaskDto } from '../models/task.model';

@Injectable({
  providedIn: 'root' 
})
export class DashboardService {
  constructor(private http: HttpClient) { }

  getDeveloperWorkload(): Observable<DeveloperWorkloadDto[]> {
    return this.http.get<DeveloperWorkloadDto[]>('dashboard/developer-workload');
  }

  getProjectHealth(): Observable<ProjectHealthDto[]> {
    return this.http.get<ProjectHealthDto[]>('dashboard/project-health');
  }

  getDeveloperDelayRisk(): Observable<DeveloperDelayRiskDto[]> {
    return this.http.get<DeveloperDelayRiskDto[]>('dashboard/developer-delay-risk');
  }

  getUpcomingTasks(days: number = 7): Observable<UpcomingTaskDto[]> {
    return this.http.get<UpcomingTaskDto[]>(`dashboard/upcoming-tasks?days=${days}`);
  }
}