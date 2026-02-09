import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';

import { TaskDto } from '../../../models/task.model';

@Component({
  selector: 'app-task-detail-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatChipsModule
  ],
  templateUrl: './task-detail.dialog.html',
  styleUrls: ['./task-detail.dialog.scss']
})
export class TaskDetailDialogComponent {
  
  constructor(@Inject(MAT_DIALOG_DATA) public data: TaskDto) {}
  
  getStatusClass(status: string): string {
    return status.toLowerCase();
  }

  getPriorityClass(priority: string): string {
    return priority.toLowerCase();
  }

  formatDate(date: Date | null): string {
    if (!date) return 'N/A';
    return new Date(date).toLocaleDateString();
  }

  isOverdue(dueDate: Date | null, status: string): boolean {
    if (!dueDate || status === 'Completed') return false;
    return new Date(dueDate) < new Date();
  }
}