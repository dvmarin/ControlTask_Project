import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

// Material imports
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';

// Services
import { DashboardService } from '../../services/dashboard.service';
import { DeveloperService } from '../../services/developer.service';

// Models
import { DeveloperWorkloadDto, DeveloperDelayRiskDto } from '../../models/developer.model';
import { ProjectHealthDto } from '../../models/project.model';


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatTableModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  // Tabla 1: Carga por desarrollador
  developerWorkload: DeveloperWorkloadDto[] = [];
  developerWorkloadColumns: string[] = ['developerName', 'openTasksCount', 'averageEstimatedComplexity'];

  // Tabla 2: Estado por proyecto
  projectHealth: ProjectHealthDto[] = [];
  projectHealthColumns: string[] = ['projectName', 'clientName', 'totalTasks', 'openTasks', 'completedTasks'];

  // Tabla 3: Riesgo de retraso
  developerDelayRisk: DeveloperDelayRiskDto[] = [];
  delayRiskColumns: string[] = ['developerName', 'openTasksCount', 'avgDelayDays', 'nearestDueDate', 'latestDueDate', 'predictedCompletionDate', 'highRiskFlag'];

  loading = {
    workload: true,
    projects: true,
    risk: true
  };

  totalOpenTasks = 0;
  highRiskDevelopers = 0;

  constructor(
    private dashboardService: DashboardService,
    private developerService: DeveloperService
  ) { }

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    // Cargar carga por desarrollador
    this.dashboardService.getDeveloperWorkload().subscribe({
      next: (data) => {
        this.developerWorkload = data;
        this.loading.workload = false;
      },
      error: (error) => {
        console.error('Error loading developer workload:', error);  //Mensaje generico con logs en el backend 
        this.loading.workload = false;
      }
    });

    // Cargar estado por proyecto
    this.dashboardService.getProjectHealth().subscribe({
      next: (data) => {
        this.projectHealth = data;
        this.totalOpenTasks = data.reduce(
          (total, project) => total + project.openTasks,
          0
        );
        this.loading.projects = false;
      },
      error: (error) => {
        console.error('Error loading project health:', error);
        this.loading.projects = false;
      }
    });

    // Cargar riesgo de retraso
    this.dashboardService.getDeveloperDelayRisk().subscribe({
      next: (data) => {
        this.developerDelayRisk = data;
        this.highRiskDevelopers = data.filter(
          dev => dev.highRiskFlag === true
        ).length;
        this.loading.risk = false;
      },
      error: (error) => {
        console.error('Error loading developer delay risk:', error);
        this.loading.risk = false;
      }
    });
  }

  // Método para determinar si una fila de proyecto necesita resaltarse
  highlightProjectRow(project: ProjectHealthDto): boolean {
    return project.openTasks > project.completedTasks;
  }

  // Método para formatear la complejidad
  formatComplexity(complexity: number): string {
    return complexity.toFixed(2);
  }

  // Método para formatear fechas
  formatDate(date: Date | null): string {
    if (!date) return 'N/A';
    return new Date(date).toLocaleDateString();
  }


}