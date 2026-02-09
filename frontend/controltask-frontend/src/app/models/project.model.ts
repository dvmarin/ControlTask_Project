export interface Project {
  projectId: number;
  name: string;
  clientName: string;
  startDate: Date;
  endDate: Date | null;
  status: 'Planned' | 'InProgress' | 'Completed';
  createdAt: Date;
  updatedAt: Date;
}

export interface ProjectDto {
  projectId: number;
  name: string;
  clientName: string;
  startDate: Date;
  endDate: Date | null;
  status: string;
  totalTasks: number;
  openTasks: number;
  completedTasks: number;
}

export interface ProjectHealthDto {
  projectId: number;
  projectName: string;
  clientName: string;
  totalTasks: number;
  openTasks: number;
  completedTasks: number;
}