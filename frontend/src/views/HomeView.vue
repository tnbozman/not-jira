<script setup lang="ts">
import Button from 'primevue/button'
import Card from 'primevue/card'
import { ref } from 'vue'

const message = ref('Welcome to Not JIRA!')
const showMessage = () => {
  alert('PrimeVue and Tailwind are working!')
}
</script>

<template>
  <div class="grid gap-6">
    <Card class="shadow-md">
      <template #title>
        <h2 class="text-2xl font-semibold text-gray-800">{{ message }}</h2>
      </template>
      <template #content>
        <p class="text-gray-600 mb-4">
          This is a Vue 3 application with PrimeVue components and Tailwind CSS styling.
        </p>
        <div class="flex gap-3">
          <Button label="Test PrimeVue" icon="pi pi-check" @click="showMessage" />
          <Button label="Secondary" severity="secondary" />
          <Button label="Success" severity="success" />
        </div>
      </template>
    </Card>

    <Card class="shadow-md">
      <template #title>
        <h3 class="text-xl font-semibold text-gray-800">Features</h3>
      </template>
      <template #content>
        <ul class="list-disc list-inside space-y-2 text-gray-600">
          <li>Vue 3 with TypeScript</li>
          <li>PrimeVue UI Components</li>
          <li>Tailwind CSS for styling</li>
          <li>.NET Backend with PostgreSQL</li>
          <li>Docker Compose for deployment</li>
          <li>DevContainer support</li>
        </ul>
      </template>
    </Card>
  </div>
</template>
