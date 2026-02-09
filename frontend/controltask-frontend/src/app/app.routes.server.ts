import { Routes } from '@angular/router';

export const routes: Routes = [
  { 
    path: '', 
    redirectTo: 'dashboard', 
    pathMatch: 'full' 
  },
  { 
    path: 'dashboard',
    loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  { 
    path: 'projects',
    loadComponent: () => import('./components/projects/projects.component').then(m => m.ProjectsComponent)
  },
  { 
    path: 'projects/:id',
    loadComponent: () => import('./components/projects/project-detail/project-detail.component').then(m => m.ProjectDetailComponent),
  },
  { 
    path: 'tasks',
    loadComponent: () => import('./components/tasks/tasks.component').then(m => m.TasksComponent)
  },
  // { 
  //   path: 'tasks/new',
  //   loadComponent: () => import('./components/tasks/task-form/task-form.component').then(m => m.TaskFormComponent)
  // },
  { 
    path: '**', 
    redirectTo: 'dashboard' 
  }
];