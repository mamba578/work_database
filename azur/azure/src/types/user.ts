import type { Order } from './order'

export interface User {
  idUser: number
  lastName: string
  firstName: string
  middleName: string
  login: string
  email: string
  phone: string
  password?: string
  idrole: number
  orders?: Order[] | null
}