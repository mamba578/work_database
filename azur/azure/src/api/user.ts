import { api } from './axios'
import type { User } from '@/types/user'

export const userApi = {
  update: async (id: number, user: any): Promise<void> => { // <--- используем any для обхода жесткой типизации объекта обновления, если нужно
    await api.put(`/User/Update/${id}`, {
      ...user,
      Name: user.Name || user.name || '',
      Role: user.Role || user.role || 'Клиент',
    })
  },
}