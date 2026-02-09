import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'priorityIcon',
  standalone: true
})
export class PriorityIconPipe implements PipeTransform {
  transform(priority: string): string {
    switch (priority?.toLowerCase()) {
      case 'high':
        return 'warning'; // Icono de advertencia
      case 'medium':
        return 'schedule'; // Icono de reloj
      case 'low':
        return 'arrow_downward'; // Icono de flecha abajo
      default:
        return 'help'; // Icono de ayuda
    }
  }
}