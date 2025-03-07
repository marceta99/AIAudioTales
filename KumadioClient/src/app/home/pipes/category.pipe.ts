import { Pipe, PipeTransform } from '@angular/core';
import { BookCategory } from 'src/app/entities';

@Pipe({
  name: 'bookCategoryPipe',
})
export class BookCategoryPipe implements PipeTransform {
  transform(category: BookCategory | number): string {
    if(typeof category === "number") {
      category = category as BookCategory
    }
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
      case BookCategory.Trending:
            return 'Trending';
      case BookCategory.Recommended:
            return 'Recommended';
      // Add other cases as needed
      default:
        return 'Unknown Category';
    }
  }
}
