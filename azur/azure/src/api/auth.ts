import { api } from './axios'
import type { User } from '@/types/user'

export const authApi = {
  login: async (login: string, password: string): Promise<User> => {
    // Передаем данные и как JSON-объект во всех возможных регистрах, 
    // и как параметры в query-строке (на случай, если контроллер принимает их через [FromQuery])
    const payload = {
      login: login,
      Login: login,
      password: password,
      Password: password
    }
    
    // Пробуем отправить POST с телом и одновременно дублируем в query-параметрах
    const res = await api.post<User>(`/User/Authorization?login=${encodeURIComponent(login)}&password=${encodeURIComponent(password)}&Login=${encodeURIComponent(login)}&Password=${encodeURIComponent(password)}`, payload)
    return res.data
  },

  register: async (data: {
    lastName: string
    firstName: string
    middleName: string
    login: string
    email: string
    phone: string
    password: string
  }): Promise<void> => {
    const payload = {
      surname: data.lastName,
      Surname: data.lastName,
      name: data.firstName,
      Name: data.firstName,
      patronymic: data.middleName,
      Patronymic: data.middleName,
      login: data.login,
      Login: data.login,
      email: data.email,
      Email: data.email,
      phone: data.phone,
      Phone: data.phone,
      password: data.password,
      Password: data.password,
      status: 'Активный',
      role: 'Клиент'
    }
    await api.post('/User/Registration', payload)
  },
}