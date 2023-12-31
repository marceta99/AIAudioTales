//Entities
export interface User{
  email: string,
  role: Role
}
export interface RegisterUser{
  email: string,
  password : string,
  confirmPassword: string
}
export interface Book{
  id: number,
  title: string,
  description: string,
  bookCategory: number,
  imageURL: string
}
export interface BooksPaginated{
  booksCategory: BookCategory,
  books: Book[],
  pageSize: number,
  pageNumber: number
}

export interface Story{
  id: number,
  title: string,
  text: string,
  bookId: number,
  audioDataUrl: string
}

//Enums
export enum Role{
  Admin="ADMIN",
  Listener="LISTENER"
}

export enum BookCategory{
  BedTime = 0,
  History = 1,
  Math = 2,
  Geography = 3,
  Nature = 4,
  Trending = 5,
  Recommended = 6

}

export enum HomePageCategories{

}
