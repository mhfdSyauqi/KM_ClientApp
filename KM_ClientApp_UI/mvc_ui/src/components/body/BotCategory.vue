<script setup>
import ButtonCategory from '@/components/body/ButtonCategory.vue'
import BotCategories from '@/shared/botCategories'

import { useContentStore } from '@/stores/content'

const contentStore = useContentStore()
const props = defineProps({
  record: {
    type: BotCategories,
    required: true
  }
})
const currSearchedId = props.record.searched_identity
const currLayer = props.record.layer
const nextLayer = props.record.layer + 1
const currPage = props.record.paginations.current
const nextPage = props.record.paginations.next
const prevPage = props.record.paginations.previous
</script>

<template>
  <div class="flex justify-start items-end" v-if="!props.record.selected">
    <div class="basis-[12%]">&nbsp;</div>
    <ul class="basis-[70%] flex flex-row flex-wrap items-start gap-2 mr-7">
      <li
        v-for="category in props.record.items"
        :key="category.id"
        @click="contentStore.SelectedCategoryContent(category, nextLayer, props.record.time)"
      >
        <ButtonCategory>
          {{ category.name }}
        </ButtonCategory>
      </li>

      <li v-if="nextPage !== null || props.record.items.length > 0">
        <ButtonCategory
          @click.prevent="
            contentStore.LoadMoreLayeredContent(currSearchedId, nextPage, props.record.time)
          "
          >More...</ButtonCategory
        >
      </li>
      <li v-if="currLayer >= 2 || currPage > 1">
        <ButtonCategory
          @click.prevent="
            contentStore.BackToContent(currSearchedId, currLayer, prevPage, props.record.time)
          "
          >Go Back</ButtonCategory
        >
      </li>
      <li v-if="currLayer > 1">
        <ButtonCategory @click.prevent="contentStore.BackToMainMenu(currLayer, props.record.time)"
          >Go Main Menu</ButtonCategory
        >
      </li>
    </ul>
  </div>
</template>

<style scoped></style>
