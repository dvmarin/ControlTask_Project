import { Injectable } from '@angular/core';
import { HttpClient,  HttpParams} from '@angular/common/http';
import { Observable } from 'rxjs';

import { TaskDto, CreateTaskDto, UpdateTaskStatusDto } from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  constructor(private http: HttpClient) { }

  getTask(id: number): Observable<TaskDto> {
    return this.http.get<TaskDto>(`tasks/${id}`);
  }

createTask(task: CreateTaskDto): Observable<any> {
  return this.http.post<any>('tasks', task); 
}

  // createTask(task: { createTaskDto: CreateTaskDto }): Observable<any> {
  //   return this.http.post<any>(`tasks`, task);
  // }

  updateTaskStatus(id: number, updateDto: UpdateTaskStatusDto): Observable<TaskDto> {
    return this.http.put<TaskDto>(`tasks/${id}/status`, updateDto);
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`tasks/${id}`);
  }
}