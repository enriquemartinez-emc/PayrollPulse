import { v4 as uuidv4 } from "uuid";
import type { ChatMessage } from "~/types";

export function useExplain() {
  const messages = ref<ChatMessage[]>([]);
  const status = ref<"error" | "submitted" | "streaming" | "ready">("ready");

  async function explain(payslipId: string, message: string) {
    messages.value.push({
      id: uuidv4(),
      role: "user",
      parts: [{ type: "text", text: message }],
    });

    status.value = "submitted";

    const response = await $fetch<ReadableStream>("/payslips/explain", {
      method: "POST",
      baseURL: useRuntimeConfig().public.apiUrl,
      headers: { Authorization: `Bearer ${useCookie("auth-token").value}` },
      body: { payslipId, message },
      responseType: "stream",
    });

    const reader = response.pipeThrough(new TextDecoderStream()).getReader();

    while (true) {
      const { value, done } = await reader.read();

      if (done) {
        status.value = "ready";
        break;
      }

      console.log("Received:", value);
      status.value = "streaming";

      if (value.startsWith("data: ")) {
        const data = value.slice(6).trimEnd();
        const last = messages.value.at(-1);
        if (!last || last.role !== "assistant") {
          messages.value.push({
            id: uuidv4(),
            role: "assistant",
            parts: [{ type: "text", text: data }],
          });
        } else {
          last.parts[0]!.text += data;
        }
      }
    }
  }

  return { status, messages, explain };
}
