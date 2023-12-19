import { Pipe, PipeTransform } from '@angular/core';
import { BookCategory } from '../entities';

@Pipe({
  name: 'bookCategoryPipe',
})
export class BookCategoryPipe implements PipeTransform {
  transform(category: BookCategory): string {
    switch (category) {
      case BookCategory.BedTime:
        return 'BedTime';
      case BookCategory.History:
        return 'History';
      case BookCategory.Geography:
        return 'Geography';
      case BookCategory.Math:
          return 'Math';
      case BookCategory.Nature:
            return 'Nature';
      // Add other cases as needed
      default:
        return 'Unknown Category';
    }
  }
}
