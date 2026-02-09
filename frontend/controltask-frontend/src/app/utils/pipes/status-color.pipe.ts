import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'statusColor',
  standalone: true
})
export class StatusColorPipe implements PipeTransform {
  transform(status: string): string {
    switch (status?.toLowerCase()) {
      case 'completed':
        return '#4caf50'; // Verde
      case 'inprogress':
        return '#2196f3'; // Azul
      case 'todo':
        return '#ff9800'; // Naranja
      case 'blocked':
        return '#f44336'; // Rojo
      case 'planned':
        return '#9c27b0'; // Púrpura
      default:
        return '#9e9e9e'; // Gris
    }
  }
}