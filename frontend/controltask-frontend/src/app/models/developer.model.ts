export interface Developer {
  developerId: number;
  firstName: string;
  lastName: string;
  email: string;
  isActive: boolean;
  createdAt: Date;
}

export interface DeveloperDto {
  developerId: number;
  fullName: string;
  email: string;
  isActive: boolean;
}

export interface DeveloperWorkloadDto {
  developerName: string;
  openTasksCount: number;
  averageEstimatedComplexity: number;
}

export interface DeveloperDelayRiskDto {
  developerName: string;
  openTasksCount: number;
  avgDelayDays: number;
  nearestDueDate: Date | null;
  latestDueDate: Date | null;
  predictedCompletionDate: Date | null;
  highRiskFlag: boolean;
}