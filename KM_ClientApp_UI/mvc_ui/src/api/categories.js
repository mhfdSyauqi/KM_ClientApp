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
