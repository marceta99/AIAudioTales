//Entities
export interface User {
  firstName: string,
  lastName: string,
  email: string,
  role: Role
}
export interface RegisterUser {
  firstName: string, 
  lastName: string,
  email: string,
  password : string,
  confirmPassword: string
}

export interface RegisterCreator {
  firstName: string, 
  lastName: string,
  email: string,
  password : string,
  confirmPassword: string
}
export interface Book {
  id: number,
  title: string,
  description: string,
  bookCategory: BookCategory,
  imageURL: string,
  categoryId: number
}

export interface PurchasedBook {
  bookId: number,
  title: string,
  description: string,
  bookCategory: BookCategory,
  imageURL: string,
  purchaseType: PurchaseType,
  language: Language,
  playingPart: ReturnPart,
  playingPosition: number,
  isBookPlaying: boolean,
  questionsActive: boolean,
  remainingTime?: number;
}
export interface SearchedBooks {
  searchTerm: string,
  books: ReturnBook[]
}

export interface ReturnBook {
  id: number;
  title: string;
  description: string;
  imageURL: string;
  categoryId: number;
}

export interface Purchase {
  bookId: number,
  purchaseType: PurchaseType,
  language: Language
}

export interface Toast {
  toastType: ToastType,
  toastIcon: ToastIcon,
  text: string;
  timeoutId?: any; // Optional property to store the timeout ID
}

export interface Category {
  id: number,
  categoryName: string,
  description: string
}

export interface BasketItem {
  id: number,
  itemPrice: number,
  bookId: number,
  book: Book
}

export interface Basket {
  basketItems: BasketItem[],
  totalPrice: number
}


export interface CreateBook {
  title: string,
  description: string,
  price: number,
  imageURL: string,
  categoryId: number
}

export interface CreateRootPart {
  bookId: number,
  partAudio: File | null,
  answers: CreateAnswer[]
}

export interface CreateAnswer {
  text: string
}

export interface CreatePart {
  bookId: number,
  partAudio: File | null,
  parentAnswerId: number,
  answers: CreateAnswer[]
}

export interface ReturnPart {
  id: number;
  partAudioLink: string;
  isRoot: boolean;
  bookId: number;
  parentAnswer?: ReturnAnswer;
  answers: ReturnAnswer[];
}

export interface PartTree {
  partId: number,
  partName: string,
  answers: ReturnAnswer[],
  nextParts: PartTree[]
}

export interface ReturnAnswer {
  id: number,
  text: string,
  currentPartId: number,
  nextPartId: number | null
}

export interface ApiMessageResponse {
  message: string
}

//Enums
export enum ToastType{
  Success = "success",
  Error = "error",
  Warning = "warning",
  Info = "info"
}
export enum ToastIcon{
  Success = "fa-circle-check",
  Error = "fa-circle-xmark",
  Warning = "fa-triangle-exclamation",
  Info = "fa-circle-info"
}

export enum Role{
  ADMIN = 0,
  LISTENER_WITH_SUBSCRIPTION = 1,
  LISTENER_NO_SUBSCRIPTION = 2,
  CREATOR = 3
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

export enum PurchaseType{
  BasicPurchase = 0,
  CustomVoice = 1,
  Enroled = 2
}

export enum Language{
  ENGLISH_USA=0,
  ENGLISH_UK=1,
  ENGLISH_AUSTRALIA=2,
  ENGLISH_CANADA=3,
  GERMAN=4,
  POLISH=5,
  SPANISH_SPAIN=6,
  SPANISH_MEXICO=7,
  ITALIAN=8,
  FRENCH_FRANCE=9,
  FRENCH_CANADA=10,
  PORTUGUESE_PORTUGAL=11,
  PORTUGUESE_BRASIL=12,
  HINDI=13
}
