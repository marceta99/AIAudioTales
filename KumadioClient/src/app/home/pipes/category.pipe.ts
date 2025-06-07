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
        return 'Uspavanke';
      case BookCategory.History:
        return 'Istorija';
      case BookCategory.Geography:
        return 'Geografija';
      case BookCategory.Math:
          return 'Matematika';
      case BookCategory.Nature:
            return 'Priroda';
      case BookCategory.Trending:
            return 'Popularno';
      case BookCategory.Recommended:
            return 'Preporuceno';
      case BookCategory.Finance:
            return 'Finansije';
      // Add other cases as needed
      default:
        return 'Unknown Category';
    }
  }
}
