class BotRecord {
  constructor(time, messages) {
    this.time = time
    this.message = {
      text: messages.text,
      type: messages.type
    }
  }
}

export default BotRecord
