import { api } from './axios'
import type { Product } from '@/types/product'

export const productsApi = {
  getAll: async (): Promise<Product[]> => {
    const res = await api.get<Product[]>('/Tovar/AllProducts')
    return res.data
  },
}