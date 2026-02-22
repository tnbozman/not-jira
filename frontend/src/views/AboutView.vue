<script setup lang="ts"></script>

<template>
  <div class="about-page">
    <div class="about-header">
      <h1 class="about-title">About Not JIRA</h1>
      <p class="about-sub">A modern project management tool built with modern technologies.</p>
    </div>

    <div class="about-card">
      <div class="about-section">
        <h2 class="about-section-title">Tech Stack</h2>
        <div class="tech-grid">
          <div class="tech-item">
            <i class="pi pi-desktop tech-icon tech-icon--blue"></i>
            <div>
              <p class="tech-label">Frontend</p>
              <p class="tech-detail">Vue 3 · PrimeVue · Tailwind CSS</p>
            </div>
          </div>
          <div class="tech-item">
            <i class="pi pi-server tech-icon tech-icon--teal"></i>
            <div>
              <p class="tech-label">Backend</p>
              <p class="tech-detail">.NET 10 · Entity Framework Core</p>
            </div>
          </div>
          <div class="tech-item">
            <i class="pi pi-database tech-icon tech-icon--amber"></i>
            <div>
              <p class="tech-label">Database</p>
              <p class="tech-detail">PostgreSQL 17</p>
            </div>
          </div>
          <div class="tech-item">
            <i class="pi pi-lock tech-icon tech-icon--purple"></i>
            <div>
              <p class="tech-label">Auth</p>
              <p class="tech-detail">Keycloak SSO · OIDC</p>
            </div>
          </div>
        </div>
      </div>

      <div class="about-section">
        <h2 class="about-section-title">Features</h2>
        <ul class="feature-list">
          <li class="feature-item">
            <i class="pi pi-check-circle feature-check"></i>
            Project management with teams and members
          </li>
          <li class="feature-item">
            <i class="pi pi-check-circle feature-check"></i>
            External entity tracking with interviews and problems
          </li>
          <li class="feature-item">
            <i class="pi pi-check-circle feature-check"></i>
            Interactive knowledge graph visualization
          </li>
          <li class="feature-item">
            <i class="pi pi-check-circle feature-check"></i>
            User story mapping with themes, epics, stories, and spikes
          </li>
          <li class="feature-item">
            <i class="pi pi-check-circle feature-check"></i>
            Sprint planning, reviews, and retrospectives
          </li>
          <li class="feature-item">
            <i class="pi pi-check-circle feature-check"></i>
            Product backlog with team, release, and epic filtering
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<style scoped>
.about-page {
  max-width: 48rem;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.about-title {
  font-size: 1.375rem;
  font-weight: 700;
  color: #0f172a;
}

.about-sub {
  font-size: 0.875rem;
  color: #64748b;
  margin-top: 0.25rem;
}

.about-card {
  background-color: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.about-section-title {
  font-size: 1rem;
  font-weight: 600;
  color: #0f172a;
  margin-bottom: 0.75rem;
}

.tech-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 0.75rem;
}

@media (min-width: 640px) {
  .tech-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

.tech-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  background-color: #f8fafc;
  border-radius: 0.5rem;
}

.tech-icon {
  font-size: 1.125rem;
}

.tech-icon--blue {
  color: #2563eb;
}
.tech-icon--teal {
  color: #0d9488;
}
.tech-icon--amber {
  color: #d97706;
}
.tech-icon--purple {
  color: #7c3aed;
}

.tech-label {
  font-size: 0.8125rem;
  font-weight: 500;
  color: #1e293b;
}

.tech-detail {
  font-size: 0.75rem;
  color: #64748b;
}

.feature-list {
  list-style: none;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.feature-item {
  display: flex;
  align-items: flex-start;
  gap: 0.5rem;
  font-size: 0.8125rem;
  color: #475569;
  line-height: 1.5;
}

.feature-check {
  color: #0d9488;
  margin-top: 0.125rem;
  flex-shrink: 0;
}
</style>
