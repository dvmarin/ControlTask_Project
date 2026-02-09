import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { TaskService } from '../../services/task.service';
import { ProjectService } from '../../services/project.service'; 
import { TaskDto } from '../../models/task.model';
// Components
import { TaskDetailDialogComponent } from './task-detail/task-detail.dialog';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatChipsModule,
    MatTooltipModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.scss']
})
export class TasksComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  projectId: number | null = null;
  tasks: TaskDto[] = [];
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

  // Paginación
  pageNumber: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  pageSizeOptions: number[] = [5, 10, 20, 50];

  // Filtros
  statusFilter: string = '';
  assigneeFilter: string = '';
  searchText: string = '';
  
  // Listas para dropdowns
  statusOptions: string[] = ['ToDo', 'InProgress', 'Completed', 'Blocked'];
  assigneeOptions: string[] = [];

  // Estado de carga
  isLoading: boolean = true;
  
  // Detalle de tarea seleccionada
  selectedTask: TaskDto | null = null;
  showTaskDetail: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private taskService: TaskService,
    private projectService: ProjectService,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.projectId = +params['projectId'];
      console.log('Project ID recibido:', this.projectId);
      
      if (this.projectId) {
        this.loadProjectTasks();
      } else {
        console.error('Error al cargar proyecto:');
      }
    });
  }

  ngAfterViewInit() {
    // Configurar sort
    if (this.sort) {
      this.sort.sortChange.subscribe(() => {
        this.pageNumber = 1;
        this.loadProjectTasks();
      });
    }
  }

  // Método para calcular total de páginas
  getTotalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  loadProjectTasks() {
    if (!this.projectId) return;
    
    this.isLoading = true;
    
    this.projectService.getProjectTasks(
      this.projectId,
      this.pageNumber,
      this.pageSize,
      this.statusFilter,
      this.assigneeFilter,
      this.searchText
    ).subscribe({
      next: (response: any) => {
        this.tasks = response.items || [];
        this.totalCount = response.totalCount || 0;
        this.pageNumber = response.pageNumber || 1;
        this.pageSize = response.pageSize || 10;
        
        // Extraer assignees únicos para el filtro
        this.extractAssignees();
        
        this.isLoading = false;
        console.log('Tareas cargadas:', this.tasks);
      },
      error: (error) => {
        console.error('Error al cargar tareas:', error);
        this.isLoading = false;
      }
    });
  }

  extractAssignees() {
    const assignees = new Set<string>();
    this.tasks.forEach(task => {
      if (task.assigneeName) {
        assignees.add(task.assigneeName);
      }
    });
    this.assigneeOptions = Array.from(assignees).sort();
  }

  // Métodos de paginación
  onPageChange(event: PageEvent) {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadProjectTasks();
  }

  // Métodos de filtro
  applyFilters() {
    this.pageNumber = 1;
    this.loadProjectTasks();
  }

  clearFilters() {
    this.statusFilter = '';
    this.assigneeFilter = '';
    this.searchText = '';
    this.pageNumber = 1;
    this.loadProjectTasks();
  }

  // Métodos para formatear y estilizar
  getStatusColor(status: string): string {
    const colors: { [key: string]: string } = {
      'ToDo': 'default',
      'InProgress': 'primary',
      'Completed': 'accent',
      'Blocked': 'warn'
    };
    return colors[status] || 'default';
  }

  getPriorityColor(priority: string): string {
    const colors: { [key: string]: string } = {
      'High': 'warn',
      'Medium': 'accent',
      'Low': 'primary'
    };
    return colors[priority] || 'default';
  }

  getComplexityColor(complexity: number): string {
    if (complexity >= 4) return 'warn';
    if (complexity >= 2) return 'accent';
    return 'primary';
  }

  formatDate(dateString: string | Date | null | undefined): string {
    if (!dateString) return 'N/A';
    
    const date = typeof dateString === 'string' ? new Date(dateString) : dateString;
    
    if (isNaN(date.getTime())) return 'Fecha inválida';
    
    return date.toLocaleDateString('es-ES', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  isOverdue(dueDate: string | Date | null | undefined): boolean {
    if (!dueDate) return false;
    
    const due = typeof dueDate === 'string' ? new Date(dueDate) : dueDate;
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    // Solo considerar vencida si no está completada
    return due < today && this.selectedTask?.status !== 'Completed';
  }

  isCompleted(task: TaskDto | null): boolean {
    return task?.status === 'Completed';
  }

  viewTaskDetails(task: TaskDto): void {
    this.dialog.open(TaskDetailDialogComponent, {
      width: '600px',
      data: task
    });
  }

  closeTaskDetail() {
    this.selectedTask = null;
    this.showTaskDetail = false;
  }

}