import { useMyFetch } from '@/shared/useMyFetch'

export async function GetContentByIdAsync(searchId) {
  const PAYLOAD = {
    Searched_Id: searchId
  }
  const { data, statusCode } = await useMyFetch(`content`).post(PAYLOAD).json()

  return {
    content: data.value?.data.content,
    is_success: statusCode.value === 200 ? true : false
  }
}
