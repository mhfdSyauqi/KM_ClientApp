<script setup>
import BaseLayout from '@/components/BaseLayout.vue'
import HeaderContent from '@/components/content/HeaderContent.vue'
import BodyContent from './components/content/BodyContent.vue'
import FooterContent from './components/content/FooterContent.vue'
</script>

<template>
  <BaseLayout>
    <template #header>
      <HeaderContent />
    </template>

    <template #body>
      <BodyContent />
    </template>

    <template #footer>
      <FooterContent />
    </template>
  </BaseLayout>
</template>

<style scoped></style>
