<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

function goToLogin() {
  router.push('/login')
}

function goToRegister() {
  router.push('/register')
}

function goToCatalogAsGuest() {
  router.push('/catalog')
}
</script>

<template>
  <div class="welcome-page">
    <!-- Фоновое изображение на весь экран -->
    <div class="welcome-background">
      <img src="@/assets/welcome-bg.png" alt="Творческие инструменты" />
    </div>

    <!-- Белый блок с контентом справа -->
    <div class="welcome-content">
      <h1 class="title">Azure</h1>

      <p class="description">
        Ваш проводник в мир премиальных инструментов для творчества:
        каллиграфии, живописи и дизайна. От кистей Ваguа до масляных красок.
        Начните свое творческое путешествие с нами прямо сейчас!
      </p>

      <div class="buttons">
        <!-- Кнопка "Войти" -->
        <button class="btn-login" @click="goToLogin">
          <svg class="icon" viewBox="0 0 24 24" fill="currentColor" width="24" height="24">
            <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/>
          </svg>
          Войти
        </button>

        <!-- Кнопка "Зарегистрироваться" -->
        <button class="btn-register" @click="goToRegister">
          <svg class="icon" viewBox="0 0 24 24" fill="currentColor" width="24" height="24">
            <path d="M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z"/>
          </svg>
          Зарегистрироваться
        </button>

        <!-- Кнопка "Войти как гость" -->
        <button class="btn-guest" @click="goToCatalogAsGuest">
          <svg class="icon" viewBox="0 0 24 24" fill="currentColor" width="20" height="20">
            <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"/>
          </svg>
          Войти как гость
        </button>
      </div>

      <!-- Точки-индикаторы (декоративные) -->
      <div class="dots">
        <span class="dot active"></span>
        <span class="dot"></span>
        <span class="dot"></span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.welcome-page {
  position: relative;
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  overflow: hidden;
  font-family: 'Segoe UI', Tahoma, sans-serif;
}

/* Фоновое изображение на весь экран */
.welcome-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 0;
}

.welcome-background img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: left center;
}

/* Белый блок с контентом справа */
.welcome-content {
  position: relative;
  z-index: 1;
  width: 50%;
  max-width: 600px;
  padding: 60px 80px;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 24px 0 0 24px;
  box-shadow: -8px 0 30px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
}

.title {
  font-size: 72px;
  font-weight: 700;
  color: #1a3a6b;
  margin: 0 0 24px 0;
  font-family: 'Georgia', serif;
  letter-spacing: 2px;
}

.description {
  font-size: 18px;
  line-height: 1.7;
  color: #444;
  text-align: center;
  max-width: 480px;
  margin: 0 0 48px 0;
}

.buttons {
  display: flex;
  flex-direction: column;
  gap: 16px;
  width: 100%;
  max-width: 380px;
}

/* Кнопка "Войти" — синяя */
.btn-login {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  padding: 18px 32px;
  background: #6F8AA6;
  color: white;
  border: none;
  border-radius: 12px;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.25s;
  box-shadow: 0 4px 12px rgba(26, 58, 107, 0.3);
}

.btn-login:hover {
  background: #357ec6;
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(26, 58, 107, 0.4);
}

/* Кнопка "Зарегистрироваться" — белая с обводкой */
.btn-register {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  padding: 18px 32px;
  background: white;
  color: #357ec6;
  border: 2px solid #357ec6;
  border-radius: 12px;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.25s;
}

.btn-register:hover {
  background: #f5f7fa;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(26, 58, 107, 0.15);
}

/* Кнопка "Войти как гость" — серая */
.btn-guest {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  padding: 14px 32px;
  background: transparent;
  color: #666;
  border: 1px solid #ddd;
  border-radius: 12px;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.25s;
  margin-top: 8px;
}

.btn-guest:hover {
  background: #f5f5f5;
  border-color: #bbb;
  color: #333;
}

.icon {
  flex-shrink: 0;
}


/* Адаптивность для маленьких экранов */
@media (max-width: 900px) {
  .welcome-page {
    justify-content: center;
  }

  .welcome-content {
    width: 90%;
    max-width: 500px;
    border-radius: 24px;
    padding: 40px 30px;
    min-height: auto;
    margin: 40px 0;
  }

  .title {
    font-size: 48px;
  }

  .description {
    font-size: 16px;
  }
}
</style>