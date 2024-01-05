class BotCategories {
  constructor(categories) {
    this.is_common = 'searched_identity' in categories
    this.searched_keyword = categories.searched_keyword
    this.searched_identity = categories.searched_identity
    this.selected = categories.selected
    this.layer = categories.layer
    this.items = categories.items
    this.paginations = categories.paginations
    this.time = categories.time
  }
}

export default BotCategories
