<script setup>
import BaseLayout from '@/components/BaseLayout.vue'
import ErrorLayout from '@/components/ErrorLayout.vue'
import BodyContent from '@/components/content/BodyContent.vue'
import FooterContent from '@/components/content/FooterContent.vue'
import HeaderContent from '@/components/content/HeaderContent.vue'

import { useConfigStore } from '@/stores/config'

import { onBeforeMount, ref } from 'vue'

const configStore = useConfigStore()
const isLoaded = ref(true)

onBeforeMount(async () => {
  await configStore.InitAppConfigAsync()

  if (configStore.appConfig === null) {
    isLoaded.value = !isLoaded.value
  }
})
</script>

<template>
  <BaseLayout v-if="isLoaded">
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

  <ErrorLayout v-else />
</template>

<style scoped></style>
