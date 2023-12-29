import { useMyFetch } from '@/shared/useMyFetch'

export async function GetCategoriesAsync(SearchedId = null, PageNum = 1) {
  const PAYLOAD = {
    Searched_Identity: SearchedId,
    Current_Page: PageNum
  }
  const { data, statusCode } = await useMyFetch(`category`).post(PAYLOAD).json()

  return {
    is_success: statusCode.value === 200 ? true : false,
    categories: data.value?.data.categories
  }
}

export async function GetReferenceCategoriesAsync(SearchedId) {
  const PAYLOAD = {
    Id: SearchedId
  }
  const { data, statusCode } = await useMyFetch(`category/ref`).post(PAYLOAD).json()

  return {
    is_success: statusCode.value === 200 ? true : false,
    reference: data.value?.data.reference_categories.ref_id
  }
}
