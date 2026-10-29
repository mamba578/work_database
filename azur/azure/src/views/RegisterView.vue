<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { authApi } from '@/api/auth'

const router = useRouter()

const form = ref({
  surname: '',
  name: '',
  patronymic: '',
  login: '',
  email: '',
  password: '',
  confirmPassword: '',
  showPassword: false,
  agree: false,
})

async function submit() {
  const f = form.value
  if (!f.surname || !f.name || !f.login || !f.email || !f.password || !f.confirmPassword) {
    alert('Заполните обязательные поля (Фамилия, Имя, Логин, Email, Пароль)'); return
  }
  if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(f.email)) {
    alert('Введите корректную почту'); return
  }
  if (f.password.length < 6) {
    alert('Пароль должен быть минимум 6 символов'); return
  }
  if (f.password !== f.confirmPassword) {
    alert('Пароли не совпадают'); return
  }
  if (!f.agree) {
    alert('Подтвердите согласие на обработку данных'); return
  }

  try {
    await authApi.register({
      lastName: f.surname,    // маппим в поля бэкенда
      firstName: f.name,
      middleName: f.patronymic || '',
      login: f.login,
      email: f.email,
      phone: '',              // если в таблице users нет телефона, оставляем пустым или передаем если нужно
      password: f.password,
    })
    alert('Регистрация успешна')
    router.push('/login')
  } catch (e: any) {
    alert(e.response?.data?.message || e.response?.data || e.message)
  }
}
</script>

<template>
  <div class="auth-wrapper">
    <div class="auth-page">
      <h1>Регистрация</h1>
      <form @submit.prevent="submit">
        <input v-model="form.surname" placeholder="Фамилия" type="text" />
        <input v-model="form.name" placeholder="Имя" type="text" />
        <input v-model="form.patronymic" placeholder="Отчество (не обязательно)" type="text" />
        <input v-model="form.login" placeholder="Логин" type="text" />
        <input v-model="form.email" type="email" placeholder="Email" />
        <input v-model="form.password" :type="form.showPassword ? 'text' : 'password'" placeholder="Пароль" />
        <input v-model="form.confirmPassword" :type="form.showPassword ? 'text' : 'password'" placeholder="Подтверждение пароля" />
        
        <label class="checkbox-label">
          <input type="checkbox" v-model="form.showPassword" /> Показать пароль
        </label>
        
        <label class="checkbox-label">
          <input type="checkbox" v-model="form.agree" /> Согласие на обработку данных
        </label>

        <button type="submit">Зарегистрироваться</button>
        <button type="button" class="back-btn" @click="router.push('/login')">Назад</button>
      </form>
    </div>
  </div>
</template>

<style scoped>
.auth-wrapper {
  min-height: 100vh;
  width: 100%;
  background-color: #DCE3EA;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
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
  gap: 12px;
  margin-top: 20px;
}

input {
  padding: 12px;
  border: 1px solid #ccc;
  border-radius: 8px;
  font-size: 15px;
  outline: none;
  transition: border-color 0.2s;
}

input:focus {
  border-color: #0066cc;
}

/* Уменьшенный шрифт для чекбоксов */
.checkbox-label {
  font-size: 12px;
  color: #666;
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
  user-select: none;
}

button {
  padding: 12px;
  border: none;
  border-radius: 8px;
  background: #6F8AA6;
  color: white;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.2s;
}

button:hover:not(:disabled) {
  background: #357ec6;
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