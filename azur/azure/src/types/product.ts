export interface Product {
  idProduct: number
  title: string
  name?: string // <-- Добавьте опциональное свойство
  price: number
  category: string
  status?: string
}