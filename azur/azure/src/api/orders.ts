import { api } from './axios'
import type { Order } from '@/types/order'

export const ordersApi = {
  getUserOrders: async (userId: number): Promise<Order[]> => {
    const res = await api.get<Order[]>(`/Order/UserOrders/${userId}`)
    return res.data
  },

  updateStatus: async (order: Order): Promise<void> => {
    await api.put('/Order/UpdateStatus', order)
  },
}