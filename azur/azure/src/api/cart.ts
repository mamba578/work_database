import { api } from './axios'
import type { Order } from '@/types/order'
import type { Cartitem } from '@/types/cartitem'

export const cartApi = {
  getActiveOrder: async (userId: number): Promise<Order> => {
    const res = await api.get<Order>(`/Order/GetActiveOrder/${userId}`)
    return res.data
  },

  createOrder: async (userId: number): Promise<Order> => {
    const res = await api.post<Order>('/Order/CreateOrder', {
      IdUser: userId,
      Status: 'Корзина',
      TotalPrice: 0,
    })
    return res.data
  },

  getByOrder: async (orderId: number): Promise<Cartitem[]> => {
    const res = await api.get<Cartitem[]>(`/OrderItems/ByOrder/${orderId}`)
    return res.data
  },

  addItem: async (item: { idOrder: number; idProduct: number; quantity: number; price: number }): Promise<void> => {
    await api.post('/OrderItems/AddItem', {
      id: 0,
      idOrder: Number(item.idOrder),
      idTovar: Number(item.idProduct),
      quantity: Number(item.quantity) || 1,
      price: Number(item.price) || 0,
      idOrderNavigation: undefined,
      idTovarNavigation: undefined
    })
  },
  deleteItem: async (itemId: number): Promise<void> => {
    await api.delete(`/OrderItems/DeleteItem/${itemId}`)
  },

  updateQuantity: async (item: Cartitem): Promise<void> => {
    await api.put(`/OrderItems/UpdateQuantity/${item.idItem}`, null, {
      params: { quantity: item.quantity }
    })
  },
}