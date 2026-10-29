import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface User {
  idUser?: number
  id?: number
  login: string
  email?: string
  phone?: string
  idrole?: number
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(JSON.parse(localStorage.getItem('user') || 'null'))

  const isAuthenticated = computed(() => !!user.value)
  const isAdmin = computed(() => user.value?.idrole === 2)

  function setUser(userData: User) {
    user.value = userData
    localStorage.setItem('user', JSON.stringify(userData))
  }

  // Добавляем недостающую функцию инициализации из localStorage
  function loadFromStorage() {
    const savedUser = localStorage.getItem('user')
    if (savedUser) {
      try {
        user.value = JSON.parse(savedUser)
      } catch (e) {
        console.error('Ошибка чтения пользователя из localStorage:', e)
      }
    }
  }

  function logout() {
    user.value = null
    localStorage.removeItem('user')
    localStorage.removeItem('cart')
  }

  // ОБЯЗАТЕЛЬНО возвращаем loadFromStorage в return!
  return {
    user,
    isAuthenticated,
    isAdmin,
    setUser,
    loadFromStorage,
    logout
  }
})