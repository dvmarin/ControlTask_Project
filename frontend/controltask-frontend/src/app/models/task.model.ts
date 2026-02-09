export interface TaskItem {
  taskId: number;
  projectId: number;
  title: string;
  description: string | null;
  assigneeId: number;
  status: 'ToDo' | 'InProgress' | 'Blocked' | 'Completed';
  priority: 'Low' | 'Medium' | 'High';
  estimatedComplexity: number | null;
  dueDate: Date | null;
  completionDate: Date | null;
  createdAt: Date;
  updatedAt: Date;
}

export interface TaskDto {
  taskId: number;
  projectId: number;
  projectName: string;
  title: string;
  description: string | null;
  assigneeId: number;
  assigneeName: string;
  status: string;
  priority: string;
  estimatedComplexity: number | null;
  dueDate: Date | null;
  completionDate: Date | null;
  createdAt: Date;
}

export interface CreateTaskDto {
  projectId: number;
  title: string;
  description?: string;
  assigneeId: number;
  status: string;
  priority: string;
  estimatedComplexity?: number;
  dueDate?: string; // ISO string
}

export interface UpdateTaskStatusDto {
  status: string;
  priority?: string;
  estimatedComplexity?: number;
}

export interface UpcomingTaskDto {
  title: string;
  projectName: string;
  assignedTo: string;
  status: string;
  priority: string;
  dueDate: Date;
  daysUntilDue: number;
}

export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}