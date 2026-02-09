import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

// Material imports
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';

// Services
import { ProjectService } from '../../../services/project.service';
import { DeveloperService } from '../../../services/developer.service';

// Models
import { ProjectDto } from '../../../models/project.model';
import { TaskDto, PagedResultDto } from '../../../models/task.model';
import { DeveloperDto } from '../../../models/developer.model';

// Components
import { TaskDetailDialogComponent } from '../../tasks/task-detail/task-detail.dialog';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatSelectModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatChipsModule,
    MatTooltipModule,
  ],
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.scss']
})
export class ProjectDetailComponent implements OnInit {
  projectId!: number;
  project: ProjectDto | null = null;
  tasks: TaskDto[] = [];
  developers: DeveloperDto[] = [];
  
  // Filtros
  selectedStatus: string = '';
  selectedAssigneeId: number | null = null;
  
  // Paginación
  pageSize = 10;
  pageIndex = 0;
  totalItems = 0;
  
  // Estados
  loading = true;
  loadingTasks = true;
  
  // Columnas de la tabla
  displayedColumns: string[] = [
    'title', 
    'assigneeName', 
    'status', 
    'priority', 
    'estimatedComplexity', 
    'createdAt', 
    'dueDate',
    'actions'
  ];
  
  // Opciones de filtro
  statusOptions = [
    { value: '', label: 'Todos' },
    { value: 'ToDo', label: 'Por Hacer' },
    { value: 'InProgress', label: 'En Progreso' },
    { value: 'Blocked', label: 'Bloqueada' },
    { value: 'Completed', label: 'Completada' }
  ];

  taskStatusData: { status: string, count: number }[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private projectService: ProjectService,
    private developerService: DeveloperService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.projectId = +params['id'];
      this.loadProject();
      this.loadDevelopers();
      this.loadTasks();
    });
  }

  loadProject(): void {
    this.loading = true;
    this.projectService.getProject(this.projectId).subscribe({
      next: (project) => {
        this.project = project;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading project:', error);
        this.loading = false;
      }
    });
  }

  loadDevelopers(): void {
    this.developerService.getActiveDevelopers().subscribe({
      next: (developers) => {
        this.developers = developers;
      },
      error: (error) => {
        console.error('Error loading developers:', error);
      }
    });
  }

  loadTasks(): void {
    this.loadingTasks = true;
    
    this.projectService.getProjectTasks(
      this.projectId,
      this.pageIndex + 1,
      this.pageSize,
      this.selectedStatus || undefined
     
    ).subscribe({
      next: (result: PagedResultDto<TaskDto>) => {
        this.tasks = result.items;
        this.totalItems = result.totalCount;
        this.loadingTasks = false;
        
        // Actualizar datos para el gráfico
        this.updateTaskStatusData(result.items);
      },
      error: (error) => {
        console.error('Error loading tasks:', error);
        this.loadingTasks = false;
      }
    });
  }

   private updateTaskStatusData(tasks: TaskDto[]): void {
    const statusCounts: { [key: string]: number } = {};
    
    tasks.forEach(task => {
      statusCounts[task.status] = (statusCounts[task.status] || 0) + 1;
    });
    
    this.taskStatusData = Object.keys(statusCounts).map(status => ({
      status,
      count: statusCounts[status]
    }));
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadTasks();
  }

  applyFilters(): void {
    this.pageIndex = 0;
    this.loadTasks();
  }

  clearFilters(): void {
    this.selectedStatus = '';
    this.selectedAssigneeId = null;
    this.pageIndex = 0;
    this.loadTasks();
  }

  viewTaskDetails(task: TaskDto): void {
    this.dialog.open(TaskDetailDialogComponent, {
      width: '600px',
      data: task
    });
  }

  navigateToNewTask(): void {
    this.router.navigate(['/tasks/new'], { 
      queryParams: { projectId: this.projectId } 
    });
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'completed': return 'completed';
      case 'inprogress': return 'inprogress';
      case 'todo': return 'todo';
      case 'blocked': return 'blocked';
      default: return '';
    }
  }

  getPriorityClass(priority: string): string {
    switch (priority.toLowerCase()) {
      case 'high': return 'high-priority';
      case 'medium': return 'medium-priority';
      case 'low': return 'low-priority';
      default: return '';
    }
  }

  formatDate(date: Date | null): string {
      if (!date) return 'N/A';
      return new Date(date).toLocaleDateString();
    }

    isOverdue(task: TaskDto): boolean {
    if (!task.dueDate || task.status === 'Completed') {
      return false;
    }

    return new Date(task.dueDate).getTime() < Date.now();
  }
}