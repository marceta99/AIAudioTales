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

//Enums
export enum Role{
  Admin="ADMIN",
  Listener="LISTENER"
}
