export interface CatalogItem {
  id: string;
  name: string;
}

export type Catalog<T extends object = {}> = CatalogItem & T;

export type Department = Catalog;
export type Policy = Catalog;
export type EmployeeItem = Catalog;

export type Employee = {
  employeeId: string;
  firstName: string;
  lastName: string;
  email: string;
  baseSalary: number;
  department: string;
  status: "Active" | "Terminated" | "OnLeave";
  hireDate: string; // ISO string (DateOnly from backend)
  terminationDate?: string;
};

export type RegisterEmployee = Omit<
  Employee,
  "employeeId" | "department" | "status" | "hireDate" | "terminationDate"
> & {
  departmentId: string;
};

export type Payroll = {
  payrollRunId: string; // Guid as string
  startDate: string; // DateOnly as ISO date string (e.g., "2025-11-11")
  endDate: string; // DateOnly as ISO date string
  totalEmployeesProcessed: number;
  totalGross: number;
  totalNet: number;
  totalEarnings: number;
  totalDeductions: number;
  createdAtUtc: string; // DateTime as ISO datetime string (e.g., "2025-11-11T10:20:00Z")
};

export type Period = {
  start: Date;
  end: Date;
};

export type PayPeriod = {
  label: string;
  value: string;
};

export interface ValidationProblemDetails {
  type: string;
  title: string;
  status: number;
  errors: Record<string, string[]>;
}

export type Payslip = {
  id: string;
  start: string;
  end: string;
  netPay: number;
  totalEarnings: number;
  totalDeductions: number;
  items: PayslipItem[];
};

export type PayslipItem = {
  compensationName: string;
  compensationType: string;
  amount: number;
  policyName: string;
  policyDescription: string;
};

export type User = {
  id: string;
  email?: string;
  role: "Admin" | "Employee";
  employeeId?: string;
};

export type UserWithPassword = User & { password: string };

export type UserToSubmit = Omit<UserWithPassword, "id">;

export type LoggedUser = {
  userId: string;
  email: string;
  role: "Admin" | "Employee";
  token: string;
  employeeId?: string;
};

type ChatRole = "user" | "assistant" | "system";

type TextPart = {
  type: "text";
  text: string;
};

export type ChatMessage = {
  id: string; // uuidv4() returns a string
  role: ChatRole; // restricts to known roles
  parts: TextPart[]; // array of parts, currently only text
};

export interface PaginatedList<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
