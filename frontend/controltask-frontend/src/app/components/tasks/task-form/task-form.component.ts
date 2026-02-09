import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

// Material imports
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTooltipModule } from '@angular/material/tooltip';

// Services
import { TaskService } from '../../../services/task.service';
import { ProjectService } from '../../../services/project.service';
import { DeveloperService } from '../../../services/developer.service';

// Models
import { CreateTaskDto } from '../../../models/task.model';
import { ProjectDto } from '../../../models/project.model';
import { DeveloperDto } from '../../../models/developer.model';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTooltipModule
  ],
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.scss']
})
export class TaskFormComponent implements OnInit {
  taskForm: FormGroup;
  isSubmitting = false;
  
  // Datos para selects
  projects: ProjectDto[] = [];
  developers: DeveloperDto[] = [];
  
  // Opciones
  statusOptions = [
    { value: 'ToDo', label: 'Por Hacer' },
    { value: 'InProgress', label: 'En Progreso' },
    { value: 'Blocked', label: 'Bloqueada' },
    { value: 'Completed', label: 'Completada' }
  ];
  
  priorityOptions = [
    { value: 'Low', label: 'Baja' },
    { value: 'Medium', label: 'Media' },
    { value: 'High', label: 'Alta' }
  ];
  
  complexityOptions = [
    { value: 1, label: '1 - Muy Simple' },
    { value: 2, label: '2 - Simple' },
    { value: 3, label: '3 - Moderada' },
    { value: 4, label: '4 - Compleja' },
    { value: 5, label: '5 - Muy Compleja' }
  ];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private taskService: TaskService,
    private projectService: ProjectService,
    private developerService: DeveloperService,
    private snackBar: MatSnackBar
  ) {
    this.taskForm = this.createForm();
  }

  ngOnInit(): void {
    this.loadProjects();
    this.loadDevelopers();
    
    // Si viene projectId por query params, setearlo
    this.route.queryParams.subscribe(params => {
      if (params['projectId']) {
        this.taskForm.patchValue({ projectId: +params['projectId'] });
      }
    });
  }

  createForm(): FormGroup {
    return this.fb.group({
      projectId: ['', Validators.required],
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.maxLength(2000)],
      assigneeId: ['', Validators.required],
      status: ['ToDo', Validators.required],
      priority: ['Medium', Validators.required],
      estimatedComplexity: [''],
      dueDate: ['']
    });
  }

  loadProjects(): void {
    this.projectService.getProjects().subscribe({
      next: (projects) => {
        this.projects = projects;
      },
      error: (error) => {
        console.error('Error loading projects:', error);
        this.showError('Error al cargar proyectos');
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
        this.showError('Error al cargar desarrolladores');
      }
    });
  }

  onSubmit(): void {
    if (this.taskForm.invalid) {
      this.markFormGroupTouched(this.taskForm);
      this.showError('Por favor complete todos los campos requeridos');
      return;
    }

    this.isSubmitting = true;
    
    const taskData: CreateTaskDto = {
      projectId: this.taskForm.value.projectId,
      title: this.taskForm.value.title,
      description: this.taskForm.value.description || undefined,
      assigneeId: this.taskForm.value.assigneeId,
      status: this.taskForm.value.status,
      priority: this.taskForm.value.priority,
      estimatedComplexity: this.taskForm.value.estimatedComplexity || undefined,
      dueDate: this.taskForm.value.dueDate || undefined
    };

    this.taskService.createTask(taskData).subscribe({
      next: (createdTask) => {
        this.isSubmitting = false;
        this.showSuccess('Tarea creada exitosamente');
        
        // Redirigir a la vista de tareas del proyecto
        this.router.navigate(['/projects', createdTask.projectId]);
      },
      error: (error) => {
        this.isSubmitting = false;
        console.error('Error creating task:', error);
        
        let errorMessage = 'Error al crear la tarea';
        if (error.status === 400 && error.error?.message) {
          errorMessage = error.error.message;
        }
        
        this.showError(errorMessage);
      }
    });
  }

  onCancel(): void {
    if (this.taskForm.value.projectId) {
      this.router.navigate(['/projects', this.taskForm.value.projectId]);
    } else {
      this.router.navigate(['/dashboard']);
    }
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  private showSuccess(message: string): void {
    this.snackBar.open(message, 'Cerrar', {
      duration: 5000,
      panelClass: ['success-snackbar']
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Cerrar', {
      duration: 5000,
      panelClass: ['error-snackbar']
    });
  }

  // Getters para validación en template
  get projectId() { return this.taskForm.get('projectId'); }
  get title() { return this.taskForm.get('title'); }
  get description() { return this.taskForm.get('description'); }
  get assigneeId() { return this.taskForm.get('assigneeId'); }
  get status() { return this.taskForm.get('status'); }
  get priority() { return this.taskForm.get('priority'); }
  get estimatedComplexity() { return this.taskForm.get('estimatedComplexity'); }
  get dueDate() { return this.taskForm.get('dueDate'); }
}