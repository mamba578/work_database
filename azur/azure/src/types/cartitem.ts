import type { Product } from './product'

export interface Cartitem {
  idItem: number
  idOrder: number
  idProduct: number
  quantity: number
  totalPrice: number
  idProductNavigation: Product
}