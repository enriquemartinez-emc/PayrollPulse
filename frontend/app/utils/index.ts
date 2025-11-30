import { format } from "date-fns";
import type { PayPeriod, Catalog } from "~/types";

export function getPayPeriodsFromOneYearBack(): PayPeriod[] {
  const periods = Array.from({ length: 12 }, (_, i) => {
    const date = new Date();
    date.setMonth(date.getMonth() - i);
    date.setDate(1);

    const start = new Date(date);
    const end = new Date(date.getFullYear(), date.getMonth() + 1, 0);

    return {
      label: `${format(start, "MMM d, yyyy")} – ${format(end, "MMM d, yyyy")}`,
      value: `${format(start, "yyyy-MM-dd")}_to_${format(end, "yyyy-MM-dd")}`,
    };
  });

  return periods;
}

export function toSelectOptions(items: Catalog[]) {
  return items.map((item) => ({
    label: item.name,
    value: item.id,
  }));
}

export function debounce(fn: Function, delay = 300) {
  let timeout: ReturnType<typeof setTimeout>;
  return (...args: any[]) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => fn(...args), delay);
  };
}
