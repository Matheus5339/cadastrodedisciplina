import type { HTMLAttributes } from "react";
import { cva, type VariantProps } from "class-variance-authority";
import { AlertCircle, CheckCircle2, Info } from "lucide-react";
import { cn } from "@/lib/utils";

const alertVariants = cva("relative w-full rounded-lg border p-4 text-sm flex items-start gap-3", {
  variants: {
    variant: {
      default: "bg-card border-border text-foreground",
      destructive: "border-destructive/40 bg-destructive/10 text-destructive",
      success: "border-success/40 bg-success/10 text-success",
    },
  },
  defaultVariants: { variant: "default" },
});

const icones = {
  default: Info,
  destructive: AlertCircle,
  success: CheckCircle2,
} as const;

export interface AlertProps extends HTMLAttributes<HTMLDivElement>, VariantProps<typeof alertVariants> {
  titulo?: string;
}

export function Alert({ className, variant = "default", titulo, children, ...props }: AlertProps) {
  const Icone = icones[variant ?? "default"];
  return (
    <div role="alert" className={cn(alertVariants({ variant }), className)} {...props}>
      <Icone className="mt-0.5 h-4 w-4 shrink-0" />
      <div>
        {titulo && <p className="mb-1 font-medium">{titulo}</p>}
        <div>{children}</div>
      </div>
    </div>
  );
}
