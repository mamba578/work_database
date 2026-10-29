<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { authApi } from '@/api/auth'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const auth = useAuthStore()

const login = ref('')
const password = ref('')
const showPassword = ref(false)
const loading = ref(false)

async function handleLogin() {
  if (!login.value || !password.value) {
    alert('Заполните все поля')
    return
  }
  loading.value = true
  try {
    const user = await authApi.login(login.value, password.value)
    auth.setUser(user)
    
    // Безопасно извлекаем логин независимо от регистра сериализации C# (camelCase или PascalCase)
    const username = (user as any).login || (user as any).Login || 'Пользователь'
    alert(`Добро пожаловать, ${username}!`)

    // Проверяем роль (поддерживаем и 'Администратор', и англоязычный вариант 'Admin' / 'Administrator')
    const role = (user as any).role || (user as any).Role
    if (role === 'Администратор' || role === 'Admin' || role === 'Administrator') {
      router.push('/admin')
    } else {
      router.push('/catalog')
    }
  } catch (e: any) {
    const status = e.response?.status
    if (status === 400 || status === 401 || status === 404) {
      alert('Неверный логин или пароль')
    } else {
      alert('Ошибка: ' + (e.response?.data?.message || e.message || 'Не удалось войти в систему'))
    }
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-wrapper">
    <div class="auth-page">
      <h1>Вход в систему</h1>
      <form @submit.prevent="handleLogin">
        <input v-model="login" placeholder="Логин" type="text" autocomplete="username" />
        <input v-model="password" :type="showPassword ? 'text' : 'password'" placeholder="Пароль" autocomplete="current-password" />
        <label class="checkbox-label">
          <input type="checkbox" v-model="showPassword" /> Показать пароль
        </label>
        <button type="submit" :disabled="loading">{{ loading ? 'Вход...' : 'Войти' }}</button>
        <button type="button" @click="router.push('/register')">Зарегистрироваться</button>
        <button type="button" class="back-btn" @click="router.push('/')">← На главную</button>
      </form>
    </div>
  </div>
</template>

<style scoped>
/* Полноэкранная обертка с новым фоном */
.auth-wrapper {
  min-height: 100vh;
  width: 100%;
  background-color: #DCE3EA;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  box-sizing: border-box; 
}

.auth-page {
  width: 100%;
  max-width: 400px;
  padding: 32px;
  background: white;
  border-radius: 16px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
}

h1 {
  font-size: 24px;
  color: #1a3a6b;
  margin: 0;
  text-align: center;
}

form {
  display: flex;
  flex-direction: column;
  gap: 14px;
  margin-top: 20px;
}

input[type="text"],
input[type="password"] {
  padding: 12px;
  border: 1px solid #ccc;
  border-radius: 8px;
  font-size: 15px;
  outline: none;
  transition: border-color 0.2s;
}

input[type="text"]:focus,
input[type="password"]:focus {
  border-color: #0066cc;
}

.checkbox-label {
  font-size: 14px;
  color: #555;
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}

button {
  padding: 12px;
  border: none;
  border-radius: 8px;
  background: #6F8AA6;
  color: rgb(255, 255, 255);
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.2s;
}

button:hover:not(:disabled) {
  background: #357ec6;
}

button:disabled {
  background: #bdbdbd;
  cursor: not-allowed;
}

.back-btn {
  background: transparent;
  color: #666;
  border: 1px solid #ccc;
}

.back-btn:hover {
  background: #f5f5f5;
  color: #333;
}
</style>